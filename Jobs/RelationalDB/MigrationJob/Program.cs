using Microsoft.EntityFrameworkCore;

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

        // TODO: Seed Data implementasyonu eklenecek
        // LocationSeeder, VeterinarianCertificationSeeder, PlatformSettingsSeeder, AnimalBreedSeeder

        Console.WriteLine("All operations completed successfully.");
    }
}