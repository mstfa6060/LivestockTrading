using Iam.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Iam.Persistence;

public class IamDbContext : DbContext
{
    public IamDbContext(DbContextOptions<IamDbContext> options) : base(options) { }

    public DbSet<User> Users => Set<User>();
    public DbSet<Role> Roles => Set<Role>();
    public DbSet<UserRole> UserRoles => Set<UserRole>();
    public DbSet<AppRefreshToken> RefreshTokens => Set<AppRefreshToken>();
    public DbSet<Country> Countries => Set<Country>();
    public DbSet<IamModule> Modules => Set<IamModule>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(IamDbContext).Assembly);
    }
}
