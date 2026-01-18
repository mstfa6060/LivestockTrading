using Common.Definitions.Domain.Entities;

namespace Common.Definitions.Infrastructure.RelationalDB;

public static class RoleRelations
{
    public static void Build(Microsoft.EntityFrameworkCore.ModelBuilder modelBuilder)
    {
        var defaultCompanyId = Guid.Parse("C9D8C846-10FC-466D-8F45-A4FA4E856ABD");

        // ════════════════════════════════════════════════════════
        // COMPANY
        // ════════════════════════════════════════════════════════
        modelBuilder.Entity<Company>().HasData(
          new Company()
          {
              Id = defaultCompanyId,
              Name = "LivestockTrading",
              IsDeleted = false,
              CreatedAt = DateTime.UtcNow
          });

        // ════════════════════════════════════════════════════════
        // ROLES
        // ════════════════════════════════════════════════════════
        modelBuilder.Entity<Role>().HasData(
            new Role()
            {
                Id = Guid.Parse("a1d5b3e4-8e5a-4b3c-9ef5-d3e5a3b7c1f8"),
                Name = "Admin",
                IsSystemRole = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                CompanyId = defaultCompanyId
            },
            new Role()
            {
                Id = Guid.Parse("b3f8a7d1-4e2c-4a3e-8b5a-d3e7b9c5e2f1"),
                Name = "User",
                IsSystemRole = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                CompanyId = defaultCompanyId
            }
        );

        // ════════════════════════════════════════════════════════
        // PERMISSIONS
        // ════════════════════════════════════════════════════════
        modelBuilder.Entity<RolePermission>().HasData(
            new RolePermission()
            {
                Id = Guid.NewGuid(),
                RoleId = Guid.Parse("a1d5b3e4-8e5a-4b3c-9ef5-d3e5a3b7c1f8"), // Admin
                Permission = "ManageUsers",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            new RolePermission()
            {
                Id = Guid.NewGuid(),
                RoleId = Guid.Parse("a1d5b3e4-8e5a-4b3c-9ef5-d3e5a3b7c1f8"), // Admin
                Permission = "ManageRoles",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            new RolePermission()
            {
                Id = Guid.NewGuid(),
                RoleId = Guid.Parse("b3f8a7d1-4e2c-4a3e-8b5a-d3e7b9c5e2f1"), // User
                Permission = "ViewListings",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            }
        );
    }
}
