using BaseModules.IAM.Domain.Entities;
using BaseModules.IAM.Infrastructure.RelationalDB;
using Common.Definitions.Domain.Entities;
using Common.Definitions.Infrastructure.RelationalDB;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;

namespace Jobs.RelationalDB.MigrationJob;

public class ApplicationDbContext : DbContext, IDefinitionDbContext
{
    public DbSet<Company> AppCompanies { get; set; }
    public DbSet<User> AppUsers { get; set; }
    public DbSet<Role> AppRoles { get; set; }
    public DbSet<UserRole> UserRoles { get; set; }
    public DbSet<Resource> AppResources { get; set; }
    public DbSet<RelRoleResource> AppRelRoleResources { get; set; }

    public DbSet<Module> AppModules { get; set; }

    public DbSet<SystemAdmin> AppSystemAdmins { get; set; }
    public DbSet<RelSystemUserModule> AppRelSystemUserModules { get; set; }
    public DbSet<RolePermission> RolePermissions { get; set; } 
    public DbSet<UserLocation> AppUserLocations { get; set; }
    public DbSet<UserPushToken> AppUserPushTokens { get; set; } 


    //AuditLog
    public DbSet<AuditLog> AppAuditLogs { get; set; }

    // Mobil Uygulama Versiyon
    public DbSet<MobilApplicationVersiyon> MobilApplicationVersiyons { get; set; }

    // Location (İl/İlçe/Mahalle)
    public DbSet<Province> Provinces { get; set; }
    public DbSet<District> Districts { get; set; }
    public DbSet<Neighborhood> Neighborhoods { get; set; }

    /// <summary>
    /// IAM
    /// </summary>
    public DbSet<AppRefreshToken> AppRefreshTokens { get; set; }
 

    // Platform Settings
    public DbSet<PlatformSetting> PlatformSettings { get; set; } 

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    { }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        CommonModelBuilder.Build(modelBuilder);
 

        modelBuilder.Entity<UserLocation>()
            .Property(x => x.Latitude)
            .HasColumnType("float");

        modelBuilder.Entity<UserLocation>()
            .Property(x => x.Longitude)
            .HasColumnType("float");

        //  UserId + CompanyId kombinasyonu benzersiz olmalı
        modelBuilder.Entity<UserLocation>()
            .HasIndex(x => new { x.UserId, x.CompanyId })
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

        // ═══════════════════════════════════════════════════════════════
        // PLATFORM SETTINGS
        // ═══════════════════════════════════════════════════════════════

        modelBuilder.Entity<PlatformSetting>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Key).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Value).IsRequired().HasMaxLength(500);
            entity.Property(e => e.DataType).IsRequired().HasMaxLength(50);
            entity.Property(e => e.Category).IsRequired().HasMaxLength(50);
            entity.Property(e => e.DisplayName).HasMaxLength(200);
            entity.Property(e => e.Description).HasMaxLength(500);
            entity.Property(e => e.MinValue).HasMaxLength(100);
            entity.Property(e => e.MaxValue).HasMaxLength(100);

            // Key unique olmalı
            entity.HasIndex(e => e.Key).IsUnique();
            entity.HasIndex(e => e.Category);
        });
 
        foreach (var foreignKey in modelBuilder.Model.GetEntityTypes()
            .SelectMany(e => e.GetForeignKeys()))
        {
            foreignKey.DeleteBehavior = DeleteBehavior.Restrict;
        }

        // Prefix LivestockTrading tables to avoid name collisions across modules
        var livestockTradingPrefix = "LivestockTrading_";
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            var clrType = entityType.ClrType;
            if (clrType == null)
                continue;

            var ns = clrType.Namespace ?? string.Empty;
            if (!ns.StartsWith("BusinessModules.LivestockTrading.Domain"))
                continue;

            // Skip entities that have explicit [Table] attribute
            var tableAttr = clrType.GetCustomAttributes(typeof(System.ComponentModel.DataAnnotations.Schema.TableAttribute), false);
            if (tableAttr.Length > 0)
                continue;

            var currentName = entityType.GetTableName();
            if (string.IsNullOrWhiteSpace(currentName))
                continue;

            if (!currentName.StartsWith(livestockTradingPrefix, StringComparison.Ordinal))
            {
                entityType.SetTableName(livestockTradingPrefix + currentName);
            }
        }

        base.OnModelCreating(modelBuilder);
    }


}
