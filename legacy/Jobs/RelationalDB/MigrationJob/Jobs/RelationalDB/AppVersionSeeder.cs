using LivestockTrading.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace MigrationJob.SeedData;

/// <summary>
/// Platform bazli varsayilan AppVersionConfig kayitlarini yukler (Android, iOS).
/// </summary>
public class AppVersionSeeder
{
    private readonly DbContext _db;

    private static readonly Guid AndroidId = new Guid("c1000000-0000-0000-0000-000000000001");
    private static readonly Guid IosId = new Guid("c1000000-0000-0000-0000-000000000002");

    public AppVersionSeeder(DbContext db)
    {
        _db = db;
    }

    public async Task SeedAsync(bool forceReseed = false)
    {
        var existing = await _db.Set<AppVersionConfig>().ToListAsync();
        var existingById = existing.ToDictionary(e => e.Id);

        if (existing.Any() && !forceReseed)
        {
            Console.WriteLine("App version config already exists. Skipping seed.");
            return;
        }

        var now = DateTime.UtcNow;
        var seedItems = new List<AppVersionConfig>
        {
            new AppVersionConfig
            {
                Id = AndroidId,
                Platform = 1, // Android
                MinSupportedVersion = "1.0.0",
                LatestVersion = "1.0.1",
                StoreUrl = "https://play.google.com/store/apps/details?id=com.livestocktrading.app",
                UpdateMessage = null,
                IsActive = true,
                CreatedAt = now,
                IsDeleted = false
            },
            new AppVersionConfig
            {
                Id = IosId,
                Platform = 2, // iOS
                MinSupportedVersion = "1.0.0",
                LatestVersion = "1.0.1",
                StoreUrl = "https://apps.apple.com/app/livestockmobile/id0000000000",
                UpdateMessage = null,
                IsActive = true,
                CreatedAt = now,
                IsDeleted = false
            }
        };

        var added = 0;
        var updated = 0;
        foreach (var item in seedItems)
        {
            if (existingById.TryGetValue(item.Id, out var current))
            {
                if (!forceReseed) continue;
                current.Platform = item.Platform;
                current.MinSupportedVersion = item.MinSupportedVersion;
                current.LatestVersion = item.LatestVersion;
                current.StoreUrl = item.StoreUrl;
                current.UpdateMessage = item.UpdateMessage;
                current.IsActive = item.IsActive;
                current.UpdatedAt = now;
                updated++;
            }
            else
            {
                await _db.Set<AppVersionConfig>().AddAsync(item);
                added++;
            }
        }

        await _db.SaveChangesAsync();
        Console.WriteLine($"  OK: {added} app version eklendi, {updated} guncellendi.");
    }
}
