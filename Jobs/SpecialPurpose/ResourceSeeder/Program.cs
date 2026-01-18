using Common.Definitions.Domain.Entities;

namespace Jobs.SpecialPurpose.ResourceSeeder;

class Program
{
    static async Task Main(string[] args)
    {
        var acceptedEnvironments = new List<string> { "production", "staging", "development" };

        if (args.Length != 1 || !acceptedEnvironments.Contains(args[0], StringComparer.OrdinalIgnoreCase))
        {
            Console.WriteLine("Error: You must specify environment; such as production, staging, or development\nProgram Exiting...");
            return;
        }

        Console.WriteLine($"Application running for {args[0]}");

        // 1) Modüller
        await Seeder.SeedAppModules(args);

        // 2) Endpoint Resource'ları
        await Seeder.SeedResources(args);

        // 3) Şirket & Roller (UPsert)
        await Seeder.SeedCompanyRoles(args);

        // 4) SystemAdmin'ler (Push notification için)
        await Seeder.SeedSystemAdmins(args);

        // 5) Rol -> Modül yetkileri (ID ile)
        var livestockTradingUserRoleId = Guid.Parse("B3F8A7D1-4E2C-4A3E-8B5A-D3E7B9C5E2F1");

        using (var db = Seeder.BuildRelationalDbContext(args))
        {
            await Seeder.AssignRolePermissionsToModule(db, livestockTradingUserRoleId, ModuleTypes.LivestockTrading);
        }

        Console.WriteLine("Seeding completed.");
        await Task.CompletedTask;
    }
}
