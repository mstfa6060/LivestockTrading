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

        // Migration (--skip-migrations ile atlanabilir)
        if (args.Contains("--skip-migrations"))
        {
            Console.WriteLine("Migration skipped (--skip-migrations flag).");
        }
        else
        {
            await dbContext.Database.MigrateAsync();
            Console.WriteLine("Migration completed successfully.");
        }

        // Atıl İlanları Yeniden Yayına Alma (seed'lerden önce çalışır, bitince çıkar)
        var reviveDryRun = args.Contains("--revive-idle-listings");
        var reviveCommit = args.Contains("--revive-idle-listings-commit");
        if (reviveDryRun || reviveCommit)
        {
            try
            {
                Console.WriteLine();
                Console.WriteLine(reviveCommit
                    ? "=== Atıl İlanları Yeniden Yayına Alma (COMMIT MODE) ==="
                    : "=== Atıl İlanları Yeniden Yayına Alma (DRY-RUN) ===");
                var reviver = new IdleListingReviver(dbContext);
                await reviver.RunAsync(commit: reviveCommit);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Revive idle listings FAILED: {ex.Message}");
                Console.WriteLine(ex.StackTrace);
                throw;
            }
            Console.WriteLine("Revive operation completed. Skipping seeders.");
            return;
        }

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

        // Seed Data - Kategoriler
        // --force-category-reseed argümani ile mevcut kategoriler güncellenebilir
        try
        {
            var forceCategoryReseed = args.Contains("--force-category-reseed");
            var categorySeeder = new CategorySeeder(dbContext);
            await categorySeeder.SeedAsync(forceCategoryReseed);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Category seed FAILED: {ex.Message}");
            throw;
        }

        // Seed Data - Para Birimleri
        try
        {
            var forceCurrencyReseed = args.Contains("--force-currency-reseed");
            var currencySeeder = new CurrencySeeder(dbContext);
            await currencySeeder.SeedAsync(forceCurrencyReseed);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Currency seed FAILED: {ex.Message}");
            throw;
        }

        // Seed Data - Abonelik Planları
        try
        {
            var forceSubscriptionReseed = args.Contains("--force-subscription-reseed");
            var subscriptionPlanSeeder = new SubscriptionPlanSeeder(dbContext);
            await subscriptionPlanSeeder.SeedAsync(forceSubscriptionReseed);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Subscription plan seed FAILED: {ex.Message}");
            throw;
        }

        // Seed Data - Boost Paketleri
        try
        {
            var forceBoostReseed = args.Contains("--force-boost-reseed");
            var boostPackageSeeder = new BoostPackageSeeder(dbContext);
            await boostPackageSeeder.SeedAsync(forceBoostReseed);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Boost package seed FAILED: {ex.Message}");
            throw;
        }

        // Seed Data - Lokasyonlar (GeoNames: Province/District)
        // --force-location-reseed argümani ile mevcut veriler silinip yeniden seed edilir
        try
        {
            var forceLocationReseed = args.Contains("--force-location-reseed");
            var locationSeeder = new LocationSeeder(dbContext);
            await locationSeeder.SeedAsync(forceLocationReseed);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Location seed FAILED: {ex.Message}");
            throw;
        }

        Console.WriteLine("All operations completed successfully.");
    }
}