using System.Text.Json;
using LivestockTrading.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace MigrationJob.SeedData;

public class BoostPackageSeeder
{
    private readonly DbContext _db;

    public BoostPackageSeeder(DbContext db)
    {
        _db = db;
    }

    public async Task SeedAsync(bool forceReseed = false)
    {
        if (await _db.Set<BoostPackage>().AnyAsync())
        {
            if (!forceReseed)
            {
                Console.WriteLine("Boost package data already exists. Skipping seed.");
                return;
            }

            Console.WriteLine("Updating existing boost package data...");
            await UpdateExistingPackagesAsync();
            return;
        }

        Console.WriteLine("Seeding boost package data...");
        await SeedPackagesAsync();
        Console.WriteLine("Boost package seed completed!");
    }

    private async Task SeedPackagesAsync()
    {
        var filePath = Path.Combine("SeedData", "boostPackages.json");
        if (!File.Exists(filePath))
        {
            Console.WriteLine($"  WARN: {filePath} not found. Skipping boost packages.");
            return;
        }

        var json = await File.ReadAllTextAsync(filePath);
        var items = JsonSerializer.Deserialize<List<BoostPackageSeedItem>>(json, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        if (items == null || items.Count == 0)
        {
            Console.WriteLine("  WARN: No boost packages found in JSON.");
            return;
        }

        var now = DateTime.UtcNow;
        var packages = items.Select(i => new BoostPackage
        {
            Id = i.Id,
            Name = i.Name,
            NameTranslations = i.NameTranslations,
            Description = i.Description,
            DescriptionTranslations = i.DescriptionTranslations,
            DurationHours = i.DurationHours,
            Price = i.Price,
            Currency = i.Currency,
            AppleProductId = i.AppleProductId,
            GoogleProductId = i.GoogleProductId,
            BoostType = (BoostType)i.BoostType,
            BoostScore = i.BoostScore,
            SortOrder = i.SortOrder,
            IsActive = i.IsActive,
            CreatedAt = now,
            IsDeleted = false
        }).ToList();

        await _db.Set<BoostPackage>().AddRangeAsync(packages);
        await _db.SaveChangesAsync();
        Console.WriteLine($"  OK: {packages.Count} boost paketi eklendi.");
    }

    private async Task UpdateExistingPackagesAsync()
    {
        var filePath = Path.Combine("SeedData", "boostPackages.json");
        if (!File.Exists(filePath))
        {
            Console.WriteLine($"  WARN: {filePath} not found. Skipping update.");
            return;
        }

        var json = await File.ReadAllTextAsync(filePath);
        var items = JsonSerializer.Deserialize<List<BoostPackageSeedItem>>(json, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        if (items == null || items.Count == 0) return;

        var existing = await _db.Set<BoostPackage>().ToListAsync();
        var map = existing.ToDictionary(p => p.Id);
        var now = DateTime.UtcNow;
        var updated = 0;

        foreach (var item in items)
        {
            if (map.TryGetValue(item.Id, out var package))
            {
                package.Name = item.Name;
                package.NameTranslations = item.NameTranslations;
                package.Description = item.Description;
                package.DescriptionTranslations = item.DescriptionTranslations;
                package.DurationHours = item.DurationHours;
                package.Price = item.Price;
                package.Currency = item.Currency;
                package.AppleProductId = item.AppleProductId;
                package.GoogleProductId = item.GoogleProductId;
                package.BoostType = (BoostType)item.BoostType;
                package.BoostScore = item.BoostScore;
                package.SortOrder = item.SortOrder;
                package.IsActive = item.IsActive;
                package.UpdatedAt = now;
                updated++;
            }
            else
            {
                await _db.Set<BoostPackage>().AddAsync(new BoostPackage
                {
                    Id = item.Id,
                    Name = item.Name,
                    NameTranslations = item.NameTranslations,
                    Description = item.Description,
                    DescriptionTranslations = item.DescriptionTranslations,
                    DurationHours = item.DurationHours,
                    Price = item.Price,
                    Currency = item.Currency,
                    AppleProductId = item.AppleProductId,
                    GoogleProductId = item.GoogleProductId,
                    BoostType = (BoostType)item.BoostType,
                    BoostScore = item.BoostScore,
                    SortOrder = item.SortOrder,
                    IsActive = item.IsActive,
                    CreatedAt = now,
                    IsDeleted = false
                });
                updated++;
            }
        }

        await _db.SaveChangesAsync();
        Console.WriteLine($"  OK: {updated} boost paketi güncellendi/eklendi.");
    }

    private class BoostPackageSeedItem
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string NameTranslations { get; set; }
        public string Description { get; set; }
        public string DescriptionTranslations { get; set; }
        public int DurationHours { get; set; }
        public double Price { get; set; }
        public string Currency { get; set; }
        public string AppleProductId { get; set; }
        public string GoogleProductId { get; set; }
        public int BoostType { get; set; }
        public int BoostScore { get; set; }
        public int SortOrder { get; set; }
        public bool IsActive { get; set; }
    }
}
