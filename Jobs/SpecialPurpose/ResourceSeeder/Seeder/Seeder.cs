using System.Reflection;
using Common.Definitions.Domain.Entities;
using Common.Definitions.Domain.Extentions;
using Common.Definitions.Infrastructure.RelationalDB;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Arfware.ArfBlocks.Core;

namespace Jobs.SpecialPurpose.ResourceSeeder;

public class ModuleDefinition
{
    public Assembly ApplicationProjectAssembly { get; set; }
    public ModuleTypes ModuleType { get; set; }
}

public class Seeder
{
    // ------------------------------------------------------------------------
    // Sabit GUID'ler - Tüm ortamlarda aynı olmalı
    // ------------------------------------------------------------------------
    public static class FixedIds
    {
        // Modules
        public static readonly Guid LivestockTradingModuleId = Guid.Parse("DFD018C9-FC32-42C4-AEFD-70A5942A295E");
        public static readonly Guid IamModuleId = Guid.Parse("BA8A94D0-5C54-4E57-9F15-34797E3171F4");
        public static readonly Guid FileProviderModuleId = Guid.Parse("72A51A33-9CE1-49CE-87FA-54863A57B977");

        // Companies
        public static readonly Guid LivestockTradingCompanyId = Guid.Parse("C9D8C846-10FC-466D-8F45-A4FA4E856ABD");

        // Roles - LivestockTrading
        public static readonly Guid LivestockTradingAdminRoleId = Guid.Parse("10000000-0000-0000-0000-000000000101");
        public static readonly Guid LivestockTradingUserRoleId = Guid.Parse("B3F8A7D1-4E2C-4A3E-8B5A-D3E7B9C5E2F1");
    }

    // ------------------------------------------------------------------------
    // DbContext Bootstrap
    // ------------------------------------------------------------------------
    public static DefinitionDbContext BuildRelationalDbContext(string[] args)
    {
        var environment = args.Length > 0 ? args[0] : "development";
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile($"appsettings.{environment}.json", optional: true, reloadOnChange: false)
            .AddEnvironmentVariables()
            .Build();

        var relationalDbOptions = DefinitionDbContext.BuildDbContextOptions(
            configuration.GetSection("RelationalDbConfiguration").Get<RelationalDbConfiguration>());

        return new DefinitionDbContext(relationalDbOptions);
    }

    // ------------------------------------------------------------------------
    // AppModules
    // ------------------------------------------------------------------------
    public static async Task SeedAppModules(string[] args)
    {
        using var db = BuildRelationalDbContext(args);

        var modulesToSeed = new List<Common.Definitions.Domain.Entities.Module>
        {
            new()
            {
                Id = FixedIds.LivestockTradingModuleId,
                Key = "livestocktrading",
                DisplayName = "LivestockTrading Platform",
                Name = "LivestockTrading",
                ModuleType = ModuleTypes.LivestockTrading,
                ModuleCategory = ModuleCategories.BusinessModule,
                IsDefault = true
            },
            new()
            {
                Id = FixedIds.IamModuleId,
                Key = "iam",
                DisplayName = "IAM Kullanıcı Yönetimi",
                Name = "IAM",
                ModuleType = ModuleTypes.IAM,
                ModuleCategory = ModuleCategories.BaseModule,
                IsDefault = true
            },
            new()
            {
                Id = FixedIds.FileProviderModuleId,
                Key = "fileprovider",
                DisplayName = "Dosya Sağlayıcı",
                Name = "FileProvider",
                ModuleType = ModuleTypes.FileProvider,
                ModuleCategory = ModuleCategories.BaseModule,
                IsDefault = true
            }
        };

        foreach (var module in modulesToSeed)
        {
            var exists = await db.AppModules.AnyAsync(m => m.ModuleType == module.ModuleType);
            if (!exists)
            {
                db.AppModules.Add(module);
                Console.WriteLine($" {module.Name} modülü eklendi.");
            }
            else
            {
                Console.WriteLine($"🔍 {module.Name} modülü zaten var, atlandı.");
            }
        }

        await db.SaveChangesAsync();
    }

    // ------------------------------------------------------------------------
    // Endpoint → Resource
    // ------------------------------------------------------------------------
    public static async Task SeedResources(string[] args)
    {
        using var db = BuildRelationalDbContext(args);

        var businessModules = new List<ModuleDefinition>
        { 
            new()
            {
                ApplicationProjectAssembly = Assembly.GetAssembly(typeof(BaseModules.IAM.Application.Configuration.ApplicationDependencyProvider)),
                ModuleType = ModuleTypes.IAM,
            },
            new()
            {
                ApplicationProjectAssembly = Assembly.GetAssembly(typeof(BaseModules.FileProvider.Application.Configuration.ApplicationDependencyProvider)),
                ModuleType = ModuleTypes.FileProvider,
            },
        };

        foreach (var module in businessModules)
        {
            Console.WriteLine("-------------------------------------");
            await SeedResourceByModule(module, db);
        }
    }

    public static async Task SeedResourceByModule(ModuleDefinition moduleDefinition, DefinitionDbContext db)
    {
        // ArfBlocks endpoint discovery
        CommandQueryRegister.RegisterAssemblyWithName(moduleDefinition.ApplicationProjectAssembly.GetName().Name);
        var endpointList = CommandQueryRegister.GetAllEndpoints();
        Console.WriteLine($"Total endpoint count: {endpointList.Count}");

        endpointList.RemoveAll(e => e.Handler.Namespace.ToLowerCase().Contains(".eventhandlers."));
        Console.WriteLine($"Total request handlers endpoint count: {endpointList.Count}");

        var module = await db.AppModules.FirstOrDefaultAsync(a => a.ModuleType == moduleDefinition.ModuleType);
        if (module == null)
        {
            Console.WriteLine($"UYARI: {moduleDefinition.ModuleType} modülü bulunamadı. AppModules tablosunda kayıt yok!");
            return;
        }

        var existing = await db.AppResources.Where(a => a.ModuleId == module.Id).ToListAsync();

        // Silinecek kaynaklar (artık karşılığı olmayan namespace)
        var deletedResources = existing
            .Where(r => !endpointList.Any(e => e.Handler.Namespace == r.Namespace))
            .ToList();

        Console.WriteLine($"{deletedResources.Count} deletedResources count");

        // FK kırılmasın: önce RelRoleResource sil
        if (deletedResources.Any())
        {
            var deletedResourceIds = deletedResources.Select(r => r.Id).ToList();
            var relatedRelRoleResources = await db.AppRelRoleResources
                .Where(r => deletedResourceIds.Contains(r.ResourceId))
                .ToListAsync();

            if (relatedRelRoleResources.Any())
            {
                db.AppRelRoleResources.RemoveRange(relatedRelRoleResources);
                await db.SaveChangesAsync();
                Console.WriteLine($"{relatedRelRoleResources.Count} adet RelRoleResource kaydı silindi.");
            }

            db.AppResources.RemoveRange(deletedResources);
            await db.SaveChangesAsync();
            existing.RemoveAll(r => deletedResources.Any(d => d.Id == r.Id));
        }

        // Yeni endpoint → resource ekle (veya adını güncelle)
        foreach (var endpoint in endpointList)
        {
            var endpointNamespace = endpoint.Handler.Namespace;
            Console.WriteLine($"Endpoint: {endpointNamespace} - {endpoint.ObjectName} - {endpoint.ActionName}");

            var matched = existing.FirstOrDefault(e => e.Namespace == endpointNamespace);
            var name = $"{endpoint.ObjectName}-{endpoint.ActionName}";

            if (matched == null)
            {
                db.AppResources.Add(new Resource
                {
                    Id = Guid.NewGuid(),
                    CreatedAt = DateTime.UtcNow,
                    Namespace = endpointNamespace,
                    Name = name,
                    IsSystemAdminPermitted = true,
                    IsCompanyAdminPermitted = true,
                    IsModuleAdminPermitted = false,
                    IsEndUserPermitted = true,
                    Title = "",
                    Description = "",
                    ModuleId = module.Id,
                });
            }
            else
            {
                matched.Name = name;
            }
        }

        await db.SaveChangesAsync();
        Console.WriteLine($"{moduleDefinition.ModuleType} seeding completed");
    }

    // ------------------------------------------------------------------------
    // Companies & Roles (Upsert)
    // ------------------------------------------------------------------------
    public static async Task SeedCompanyRoles(string[] args)
    {
        using var db = BuildRelationalDbContext(args);

        // Companies (ensure)
        var livestockTradingCompany = await db.AppCompanies.FirstOrDefaultAsync(c => c.Id == FixedIds.LivestockTradingCompanyId);
        if (livestockTradingCompany == null)
        {
            livestockTradingCompany = new Company
            {
                Id = FixedIds.LivestockTradingCompanyId,
                Name = "LivestockTrading",
                IsDeleted = false,
                CreatedAt = DateTime.UtcNow
            };
            db.AppCompanies.Add(livestockTradingCompany);
            Console.WriteLine("LivestockTrading Company eklendi.");
        }

        await db.SaveChangesAsync();

        // LivestockTrading Roles
        await UpsertRole(db, FixedIds.LivestockTradingAdminRoleId, "Admin", FixedIds.LivestockTradingCompanyId, true);
        await UpsertRole(db, FixedIds.LivestockTradingUserRoleId, "User", FixedIds.LivestockTradingCompanyId, false);

        await db.SaveChangesAsync();
    }

    /// <summary>
    /// Rol upsert helper metodu
    /// </summary>
    private static async Task UpsertRole(DefinitionDbContext db, Guid roleId, string roleName, Guid companyId, bool isSystemRole)
    {
        var existingRole = await db.AppRoles.FirstOrDefaultAsync(r => r.Id == roleId);
        if (existingRole == null)
        {
            db.AppRoles.Add(new Role
            {
                Id = roleId,
                Name = roleName,
                CompanyId = companyId,
                IsSystemRole = isSystemRole,
                IsDeleted = false,
                CreatedAt = DateTime.UtcNow
            });
            Console.WriteLine($" {roleName} rolü sabit ID ile eklendi.");
        }
        else
        {
            var dirty = false;
            if (existingRole.Name != roleName) { existingRole.Name = roleName; dirty = true; }
            if (existingRole.CompanyId != companyId) { existingRole.CompanyId = companyId; dirty = true; }
            if (existingRole.IsDeleted) { existingRole.IsDeleted = false; dirty = true; }
            if (existingRole.IsSystemRole != isSystemRole) { existingRole.IsSystemRole = isSystemRole; dirty = true; }
            if (dirty) Console.WriteLine($"↻ {roleName} rolü güncellendi.");
        }
    }

    // ------------------------------------------------------------------------
    // SystemAdmins (Push notification için)
    // ------------------------------------------------------------------------
    public static async Task SeedSystemAdmins(string[] args)
    {
        using var db = BuildRelationalDbContext(args);

        // Admin yapılacak email listesi
        var adminEmails = new[] { "madentechnology@gmail.com", "coskun.b@outlook.com" };

        foreach (var email in adminEmails)
        {
            // Kullanici var mi kontrol et
            var user = await db.AppUsers.FirstOrDefaultAsync(u => u.Email == email);
            if (user == null)
            {
                Console.WriteLine($"Kullanici bulunamadi, atlaniyor: {email}");
                continue;
            }

            // Zaten SystemAdmin mi kontrol et
            var existingAdmin = await db.AppSystemAdmins.AnyAsync(a => a.UserId == user.Id);
            if (existingAdmin)
            {
                Console.WriteLine($"Zaten SystemAdmin: {email}");
                continue;
            }

            // SystemAdmin ekle
            db.AppSystemAdmins.Add(new SystemAdmin
            {
                Id = Guid.NewGuid(),
                UserId = user.Id,
                IsActive = true,
                IsAllModulePermitted = true
            });

            Console.WriteLine($"SystemAdmin eklendi: {email}");
        }

        await db.SaveChangesAsync();
    }

    // ------------------------------------------------------------------------
    // Role -> Module Resource Permissions (by ROLE ID)
    // ------------------------------------------------------------------------
    public static async Task AssignRolePermissionsToModule(
        DefinitionDbContext db,
        Guid roleId,
        ModuleTypes moduleType)
    {
        var role = await db.AppRoles.FirstOrDefaultAsync(r => r.Id == roleId && !r.IsDeleted);
        if (role == null)
        {
            Console.WriteLine($"⚠️ Role not found by Id: {roleId}");
            return;
        }

        var module = await db.AppModules.FirstOrDefaultAsync(m => m.ModuleType == moduleType);
        if (module == null)
        {
            Console.WriteLine($"⚠️ Module not found: {moduleType}");
            return;
        }

        var resourceIds = await db.AppResources
            .Where(r => r.ModuleId == module.Id)
            .Select(r => r.Id)
            .ToListAsync();

        if (resourceIds.Count == 0)
        {
            Console.WriteLine($"ℹ️ No resources for module {moduleType}.");
            return;
        }

        var existing = await db.AppRelRoleResources
            .Where(x => x.RoleId == role.Id && resourceIds.Contains(x.ResourceId))
            .Select(x => x.ResourceId)
            .ToListAsync();

        var toAdd = resourceIds
            .Except(existing)
            .Select(rid => new RelRoleResource
            {
                Id = Guid.NewGuid(),
                RoleId = role.Id,
                ResourceId = rid
            })
            .ToList();

        if (toAdd.Any())
        {
            db.AppRelRoleResources.AddRange(toAdd);
            await db.SaveChangesAsync();
            Console.WriteLine($" {moduleType} için '{role.Name}' rolüne {toAdd.Count} yetki eklendi.");
        }
        else
        {
            Console.WriteLine($"🔍 {moduleType} için '{role.Name}' rolünde eklenecek yeni yetki yok.");
        }
    }
}