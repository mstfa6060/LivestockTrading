using System.Text.Json;
using LivestockTrading.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace MigrationJob.SeedData;

public class SubscriptionPlanSeeder
{
    private readonly DbContext _db;

    public SubscriptionPlanSeeder(DbContext db)
    {
        _db = db;
    }

    public async Task SeedAsync(bool forceReseed = false)
    {
        if (await _db.Set<SubscriptionPlan>().AnyAsync())
        {
            if (!forceReseed)
            {
                Console.WriteLine("Subscription plan data already exists. Skipping seed.");
                return;
            }

            Console.WriteLine("Updating existing subscription plan data...");
            await UpdateExistingPlansAsync();
            return;
        }

        Console.WriteLine("Seeding subscription plan data...");
        await SeedPlansAsync();
        Console.WriteLine("Subscription plan seed completed!");
    }

    private async Task SeedPlansAsync()
    {
        var filePath = Path.Combine("SeedData", "subscriptionPlans.json");
        if (!File.Exists(filePath))
        {
            Console.WriteLine($"  WARN: {filePath} not found. Skipping subscription plans.");
            return;
        }

        var json = await File.ReadAllTextAsync(filePath);
        var items = JsonSerializer.Deserialize<List<SubscriptionPlanSeedItem>>(json, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        if (items == null || items.Count == 0)
        {
            Console.WriteLine("  WARN: No subscription plans found in JSON.");
            return;
        }

        var now = DateTime.UtcNow;
        var plans = items.Select(i => new SubscriptionPlan
        {
            Id = i.Id,
            Name = i.Name,
            NameTranslations = i.NameTranslations,
            Description = i.Description,
            DescriptionTranslations = i.DescriptionTranslations,
            TargetType = (SubscriptionTargetType)i.TargetType,
            Tier = (SubscriptionTier)i.Tier,
            PriceMonthly = i.PriceMonthly,
            PriceYearly = i.PriceYearly,
            Currency = i.Currency,
            AppleProductIdMonthly = i.AppleProductIdMonthly,
            AppleProductIdYearly = i.AppleProductIdYearly,
            GoogleProductIdMonthly = i.GoogleProductIdMonthly,
            GoogleProductIdYearly = i.GoogleProductIdYearly,
            MaxActiveListings = i.MaxActiveListings,
            MaxPhotosPerListing = i.MaxPhotosPerListing,
            MonthlyBoostCredits = i.MonthlyBoostCredits,
            HasDetailedAnalytics = i.HasDetailedAnalytics,
            HasPrioritySupport = i.HasPrioritySupport,
            HasFeaturedBadge = i.HasFeaturedBadge,
            SortOrder = i.SortOrder,
            IsActive = i.IsActive,
            CreatedAt = now,
            IsDeleted = false
        }).ToList();

        await _db.Set<SubscriptionPlan>().AddRangeAsync(plans);
        await _db.SaveChangesAsync();
        Console.WriteLine($"  OK: {plans.Count} abonelik planı eklendi.");
    }

    private async Task UpdateExistingPlansAsync()
    {
        var filePath = Path.Combine("SeedData", "subscriptionPlans.json");
        if (!File.Exists(filePath))
        {
            Console.WriteLine($"  WARN: {filePath} not found. Skipping update.");
            return;
        }

        var json = await File.ReadAllTextAsync(filePath);
        var items = JsonSerializer.Deserialize<List<SubscriptionPlanSeedItem>>(json, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        if (items == null || items.Count == 0) return;

        var existing = await _db.Set<SubscriptionPlan>().ToListAsync();
        var map = existing.ToDictionary(p => p.Id);
        var now = DateTime.UtcNow;
        var updated = 0;

        foreach (var item in items)
        {
            if (map.TryGetValue(item.Id, out var plan))
            {
                plan.Name = item.Name;
                plan.NameTranslations = item.NameTranslations;
                plan.Description = item.Description;
                plan.DescriptionTranslations = item.DescriptionTranslations;
                plan.TargetType = (SubscriptionTargetType)item.TargetType;
                plan.Tier = (SubscriptionTier)item.Tier;
                plan.PriceMonthly = item.PriceMonthly;
                plan.PriceYearly = item.PriceYearly;
                plan.Currency = item.Currency;
                plan.AppleProductIdMonthly = item.AppleProductIdMonthly;
                plan.AppleProductIdYearly = item.AppleProductIdYearly;
                plan.GoogleProductIdMonthly = item.GoogleProductIdMonthly;
                plan.GoogleProductIdYearly = item.GoogleProductIdYearly;
                plan.MaxActiveListings = item.MaxActiveListings;
                plan.MaxPhotosPerListing = item.MaxPhotosPerListing;
                plan.MonthlyBoostCredits = item.MonthlyBoostCredits;
                plan.HasDetailedAnalytics = item.HasDetailedAnalytics;
                plan.HasPrioritySupport = item.HasPrioritySupport;
                plan.HasFeaturedBadge = item.HasFeaturedBadge;
                plan.SortOrder = item.SortOrder;
                plan.IsActive = item.IsActive;
                plan.UpdatedAt = now;
                updated++;
            }
            else
            {
                await _db.Set<SubscriptionPlan>().AddAsync(new SubscriptionPlan
                {
                    Id = item.Id,
                    Name = item.Name,
                    NameTranslations = item.NameTranslations,
                    Description = item.Description,
                    DescriptionTranslations = item.DescriptionTranslations,
                    TargetType = (SubscriptionTargetType)item.TargetType,
                    Tier = (SubscriptionTier)item.Tier,
                    PriceMonthly = item.PriceMonthly,
                    PriceYearly = item.PriceYearly,
                    Currency = item.Currency,
                    AppleProductIdMonthly = item.AppleProductIdMonthly,
                    AppleProductIdYearly = item.AppleProductIdYearly,
                    GoogleProductIdMonthly = item.GoogleProductIdMonthly,
                    GoogleProductIdYearly = item.GoogleProductIdYearly,
                    MaxActiveListings = item.MaxActiveListings,
                    MaxPhotosPerListing = item.MaxPhotosPerListing,
                    MonthlyBoostCredits = item.MonthlyBoostCredits,
                    HasDetailedAnalytics = item.HasDetailedAnalytics,
                    HasPrioritySupport = item.HasPrioritySupport,
                    HasFeaturedBadge = item.HasFeaturedBadge,
                    SortOrder = item.SortOrder,
                    IsActive = item.IsActive,
                    CreatedAt = now,
                    IsDeleted = false
                });
                updated++;
            }
        }

        await _db.SaveChangesAsync();
        Console.WriteLine($"  OK: {updated} abonelik planı güncellendi/eklendi.");
    }

    private class SubscriptionPlanSeedItem
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string NameTranslations { get; set; }
        public string Description { get; set; }
        public string DescriptionTranslations { get; set; }
        public int TargetType { get; set; }
        public int Tier { get; set; }
        public decimal PriceMonthly { get; set; }
        public decimal PriceYearly { get; set; }
        public string Currency { get; set; }
        public string AppleProductIdMonthly { get; set; }
        public string AppleProductIdYearly { get; set; }
        public string GoogleProductIdMonthly { get; set; }
        public string GoogleProductIdYearly { get; set; }
        public int MaxActiveListings { get; set; }
        public int MaxPhotosPerListing { get; set; }
        public int MonthlyBoostCredits { get; set; }
        public bool HasDetailedAnalytics { get; set; }
        public bool HasPrioritySupport { get; set; }
        public bool HasFeaturedBadge { get; set; }
        public int SortOrder { get; set; }
        public bool IsActive { get; set; }
    }
}
