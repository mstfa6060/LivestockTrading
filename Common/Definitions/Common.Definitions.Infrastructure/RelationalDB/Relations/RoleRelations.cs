using Common.Definitions.Domain.Entities;

namespace Common.Definitions.Infrastructure.RelationalDB;

public static class RoleRelations
{
    public static void Build(Microsoft.EntityFrameworkCore.ModelBuilder modelBuilder)
    {
        var defaultCompanyId = Guid.Parse("C9D8C846-10FC-466D-8F45-A4FA4E856ABD");
        var animalMarketCompanyId = Guid.Parse("9DAE9CBD-82B1-4EAD-BD2B-9C5FE5146A2A");
        var keepzoCompanyId = Guid.Parse("07fad52f-9279-42f6-9a90-e193be532c1a");

        // ════════════════════════════════════════════════════════
        // COMPANY
        // ════════════════════════════════════════════════════════
        modelBuilder.Entity<Company>().HasData(
          new Company()
          {
              Id = defaultCompanyId,
              Name = "Default Company",
              IsDeleted = false,
              CreatedAt = DateTime.UtcNow
          },
          new Company()
          {
              Id = animalMarketCompanyId,
              Name = "Animal Market",
              IsDeleted = false,
              CreatedAt = DateTime.UtcNow
          },
          new Company()
          {
              Id = keepzoCompanyId,
              Name = "Keepzo",
              IsDeleted = false,
              CreatedAt = DateTime.UtcNow
          });

        // ════════════════════════════════════════════════════════
        // ROLLER
        // ════════════════════════════════════════════════════════
        modelBuilder.Entity<Role>().HasData(
            // ─────────────────────────────────────────────────────
            // Default Company Rolleri (Hirovo için)
            // ─────────────────────────────────────────────────────
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
            },
            new Role()
            {
                Id = Guid.Parse("58497685-797d-442c-8f15-1c1b7c37bdcd"),
                Name = "Employer",
                IsSystemRole = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                CompanyId = defaultCompanyId
            },

            // ─────────────────────────────────────────────────────
            // AnimalMarket Rolleri
            // ─────────────────────────────────────────────────────
            new Role()
            {
                Id = Guid.Parse("10000000-0000-0000-0000-000000000001"),
                Name = "Admin",
                IsSystemRole = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                CompanyId = defaultCompanyId // System Admin - Default Company'de kalıyor
            },
            new Role()
            {
                Id = Guid.Parse("10000000-0000-0000-0000-000000000002"),
                Name = "User",
                IsSystemRole = false,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                CompanyId = animalMarketCompanyId
            },
            new Role()
            {
                Id = Guid.Parse("10000000-0000-0000-0000-000000000003"),
                Name = "Veterinarian",
                IsSystemRole = false,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                CompanyId = animalMarketCompanyId
            },
            new Role()
            {
                Id = Guid.Parse("10000000-0000-0000-0000-000000000004"),
                Name = "Farmer",
                IsSystemRole = false,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                CompanyId = animalMarketCompanyId
            },
            new Role()
            {
                Id = Guid.Parse("10000000-0000-0000-0000-000000000005"),
                Name = "Transporter",
                IsSystemRole = false,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                CompanyId = animalMarketCompanyId
            }
        );

        // ════════════════════════════════════════════════════════
        // PERMİSSIONLAR
        // ════════════════════════════════════════════════════════
        modelBuilder.Entity<RolePermission>().HasData(
            // ─────────────────────────────────────────────────────
            // Default Company Permissions (Hirovo)
            // ─────────────────────────────────────────────────────
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
                Permission = "PostJobs",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            new RolePermission()
            {
                Id = Guid.NewGuid(),
                RoleId = Guid.Parse("b3f8a7d1-4e2c-4a3e-8b5a-d3e7b9c5e2f1"), // User
                Permission = "ViewWorkers",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },

            // ─────────────────────────────────────────────────────
            // AnimalMarket System Admin Permissions
            // ─────────────────────────────────────────────────────
            new RolePermission()
            {
                Id = Guid.Parse("10000001-0000-0000-0000-000000000001"),
                RoleId = Guid.Parse("10000000-0000-0000-0000-000000000001"),
                Permission = "AnimalMarket.Admin",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            new RolePermission()
            {
                Id = Guid.Parse("10000001-0000-0000-0000-000000000002"),
                RoleId = Guid.Parse("10000000-0000-0000-0000-000000000001"),
                Permission = "AnimalMarket.ManageUsers",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },

            // ─────────────────────────────────────────────────────
            // AnimalMarket User Permissions
            // ─────────────────────────────────────────────────────
            new RolePermission()
            {
                Id = Guid.Parse("10000002-0000-0000-0000-000000000001"),
                RoleId = Guid.Parse("10000000-0000-0000-0000-000000000002"),
                Permission = "AnimalMarket.ViewListings",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },

            // ─────────────────────────────────────────────────────
            // Veterinarian Permissions
            // ─────────────────────────────────────────────────────
            new RolePermission()
            {
                Id = Guid.Parse("10000003-0000-0000-0000-000000000001"),
                RoleId = Guid.Parse("10000000-0000-0000-0000-000000000003"),
                Permission = "AnimalMarket.Vet.CreateListing",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            new RolePermission()
            {
                Id = Guid.Parse("10000003-0000-0000-0000-000000000002"),
                RoleId = Guid.Parse("10000000-0000-0000-0000-000000000003"),
                Permission = "AnimalMarket.Vet.ApproveAnimals",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },

            // ─────────────────────────────────────────────────────
            // Farmer Permissions
            // ─────────────────────────────────────────────────────
            new RolePermission()
            {
                Id = Guid.Parse("10000004-0000-0000-0000-000000000001"),
                RoleId = Guid.Parse("10000000-0000-0000-0000-000000000004"),
                Permission = "AnimalMarket.Farmer.CreateListing",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            new RolePermission()
            {
                Id = Guid.Parse("10000004-0000-0000-0000-000000000002"),
                RoleId = Guid.Parse("10000000-0000-0000-0000-000000000004"),
                Permission = "AnimalMarket.Farmer.ApproveBids",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            new RolePermission()
            {
                Id = Guid.Parse("10000004-0000-0000-0000-000000000003"),
                RoleId = Guid.Parse("10000000-0000-0000-0000-000000000004"),
                Permission = "AnimalMarket.Farmer.ManageFarms",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },

            // ─────────────────────────────────────────────────────
            // Transporter Permissions
            // ─────────────────────────────────────────────────────
            new RolePermission()
            {
                Id = Guid.Parse("10000005-0000-0000-0000-000000000001"),
                RoleId = Guid.Parse("10000000-0000-0000-0000-000000000005"),
                Permission = "AnimalMarket.Transporter.ViewDeliveries",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            new RolePermission()
            {
                Id = Guid.Parse("10000005-0000-0000-0000-000000000002"),
                RoleId = Guid.Parse("10000000-0000-0000-0000-000000000005"),
                Permission = "AnimalMarket.Transporter.UpdateDeliveryStatus",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            }
        );
    }
}