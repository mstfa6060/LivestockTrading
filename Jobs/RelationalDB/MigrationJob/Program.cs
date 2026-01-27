using Microsoft.EntityFrameworkCore;
using MigrationJob.SeedData;

namespace Jobs.RelationalDB.MigrationJob;

class Program
{
    static async Task Main(string[] args)
    {
        var acceptedEnvironments = new List<string>() { "production", "staging", "development" };

        if (args.Length < 1 || !acceptedEnvironments.Contains(args[0]))
        {
            Console.WriteLine("Error: You must specify environment; such as production, staging, or development\nProgram Exiting...");
            return;
        }

        Console.WriteLine($"Application running for {args[0]}");

        var dbContext = new ApplicationDbContextFactory().CreateDbContext(args);

        // Bağlantı kontrolü
        try
        {
            dbContext.Database.CanConnect();
            Console.WriteLine("DB connect OK");
        }
        catch (Exception ex)
        {
            Console.WriteLine("DB connect FAILED: " + ex.Message);
            throw;
        }

        // Migration
        await dbContext.Database.MigrateAsync();
        Console.WriteLine("Migration completed successfully.");

        // Seed Data - Ulkeler
        // --force-country-reseed argümani ile mevcut veriler silinip yeniden seed edilir
        try
        {
            var forceReseed = args.Contains("--force-country-reseed");
            var countrySeeder = new CountrySeeder(dbContext);
            await countrySeeder.SeedAsync(forceReseed);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Country seed FAILED: {ex.Message}");
            throw;
        }

        // Seed Data - Roller
        // --force-role-reseed argümani ile mevcut roller güncellenebilir
        try
        {
            var forceRoleReseed = args.Contains("--force-role-reseed");
            var roleSeeder = new RoleSeeder(dbContext);
            await roleSeeder.SeedAsync(forceRoleReseed);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Role seed FAILED: {ex.Message}");
            throw;
        }

        // TODO: Diger Seed Data implementasyonlari eklenecek
        // LocationSeeder, PlatformSettingsSeeder, vb.

        Console.WriteLine("All operations completed successfully.");
    }
}