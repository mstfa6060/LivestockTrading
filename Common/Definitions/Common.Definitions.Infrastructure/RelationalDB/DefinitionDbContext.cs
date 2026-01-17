using Common.Definitions.Base.Entity;
using Common.Definitions.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Definitions.Infrastructure.RelationalDB;

public class DefinitionDbContext : DbContext, IDefinitionDbContext
{
    // User-Groups
    public DbSet<User> AppUsers { get; set; }
    public DbSet<Role> AppRoles { get; set; }
    public DbSet<UserRole> UserRoles { get; set; }
    public DbSet<Resource> AppResources { get; set; }
    public DbSet<SystemAdmin> AppSystemAdmins { get; set; }
    public DbSet<RelSystemUserModule> AppRelSystemUserModules { get; set; }
    public DbSet<RolePermission> RolePermissions { get; set; }
    public DbSet<RelRoleResource> AppRelRoleResources { get; set; }
    public DbSet<Company> AppCompanies { get; set; }
    public DbSet<UserLocation> AppUserLocations { get; set; }
    public DbSet<Module> AppModules { get; set; }
    public DbSet<UserPushToken> AppUserPushTokens { get; set; }

    //AuditLog
    public DbSet<AuditLog> AppAuditLogs { get; set; }

    // Mobil Uygulama Versiyon
    public DbSet<MobilApplicationVersiyon> MobilApplicationVersiyons { get; set; }

    // Location (İl/İlçe/Mahalle)
    public DbSet<Province> Provinces { get; set; }
    public DbSet<District> Districts { get; set; }
    public DbSet<Neighborhood> Neighborhoods { get; set; }

    public DefinitionDbContext(DefinitionDbContextOptions customDbContextOptions) : base(customDbContextOptions.DbContextOptions)
    {
        AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
    }

    public DefinitionDbContext(DbContextOptions<DefinitionDbContext> options) : base(options)
    { }

    public static DbContextOptions<DefinitionDbContext> BuildDbContextOptions(RelationalDbConfiguration configuration)
    {
        var dbContextOptionsBuilder = new DbContextOptionsBuilder<DefinitionDbContext>();
        dbContextOptionsBuilder
            .UseSqlServer(configuration.ConnectionString);

        return dbContextOptionsBuilder.Options;
    }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        CommonModelBuilder.Build(modelBuilder);

        modelBuilder.Entity<Module>().ToTable("AppModules");

        // 👇 İlişki tanımı
        modelBuilder.Entity<User>()
        .HasOne(u => u.Company)
        .WithMany(c => c.Users) // <-- BUNU EKLE
        .HasForeignKey(u => u.CompanyId)
        .OnDelete(DeleteBehavior.Restrict);

        //  UserId + CompanyId unique constraint
        modelBuilder.Entity<UserLocation>()
            .HasIndex(x => new { x.UserId, x.CompanyId })
            .IsUnique();

        modelBuilder.Entity<User>()
            .HasIndex(x => new { x.Email, x.PhoneNumber, x.CompanyId })
            .IsUnique();

        // ═══════════════════════════════════════════════════════════════
        // LOCATION TABLES (İl/İlçe/Mahalle)
        // ═══════════════════════════════════════════════════════════════

        // Province (İl)
        modelBuilder.Entity<Province>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).ValueGeneratedNever(); // Seed data için
            entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Code).IsRequired().HasMaxLength(3);
            entity.HasIndex(e => e.Code).IsUnique();
            entity.HasIndex(e => e.Name);
        });

        // District (İlçe)
        modelBuilder.Entity<District>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).ValueGeneratedNever(); // Seed data için
            entity.Property(e => e.Name).IsRequired().HasMaxLength(100);

            entity.HasOne(d => d.Province)
                .WithMany(p => p.Districts)
                .HasForeignKey(d => d.ProvinceId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasIndex(e => e.ProvinceId);
            entity.HasIndex(e => e.Name);
        });

        // Neighborhood (Mahalle)
        modelBuilder.Entity<Neighborhood>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).ValueGeneratedNever(); // Seed data için
            entity.Property(e => e.Name).IsRequired().HasMaxLength(150);
            entity.Property(e => e.PostalCode).HasMaxLength(10);

            entity.HasOne(n => n.District)
                .WithMany(d => d.Neighborhoods)
                .HasForeignKey(n => n.DistrictId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasIndex(e => e.DistrictId);
            entity.HasIndex(e => e.Name);
        });

        base.OnModelCreating(modelBuilder);
    }




    public override int SaveChanges()
    {
        AddTimestamps();
        return base.SaveChanges();
    }

    public override int SaveChanges(bool acceptAllChangesOnSuccess)
    {
        AddTimestamps();
        return base.SaveChanges(acceptAllChangesOnSuccess);
    }

    public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default(CancellationToken))
    {
        AddTimestamps();
        return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default(CancellationToken))
    {
        AddTimestamps();
        return await base.SaveChangesAsync(cancellationToken);
    }

    private void AddTimestamps()
    {
        var changedEntities = ChangeTracker.Entries()
                                            .Where(x => x.Entity is BaseEntity && (x.State == EntityState.Added || x.State == EntityState.Modified));

        var utcNow = DateTime.UtcNow; // Tek seferde al, tutarlılık için

        foreach (var entity in changedEntities)
        {
            var baseEntity = (BaseEntity)entity.Entity;
            if (entity.State == EntityState.Added)
            {
                baseEntity.SetCreatedAt(utcNow);
            }
            else if (entity.State == EntityState.Modified)
            {
                var rowNumberProperty = entity.Properties.FirstOrDefault(p => p.Metadata.Name == "RowNumber");
                if (rowNumberProperty != null)
                {
                    rowNumberProperty.IsModified = false;
                }
                baseEntity.UpdatedAt = utcNow;
                if (baseEntity.IsDeleted && !baseEntity.DeletedAt.HasValue)
                    baseEntity.DeletedAt = utcNow;
                else if (!baseEntity.IsDeleted && baseEntity.DeletedAt.HasValue)
                    baseEntity.DeletedAt = null;
            }
        }
    }
}
