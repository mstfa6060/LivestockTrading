using System.Text.Json;
using System.Text.Json.Serialization;
using LivestockTrading.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace MigrationJob.SeedData;

/// <summary>
/// Para birimi seed verilerini veritabanına yükler.
/// ExchangeRateToUSD değerleri yaklaşık başlangıç değerleridir;
/// HangfireScheduler/UpdateExchangeRatesJob her 6 saatte bir güncel değerleri çeker.
/// </summary>
public class CurrencySeeder
{
    private readonly DbContext _db;

    public CurrencySeeder(DbContext db)
    {
        _db = db;
    }

    public async Task SeedAsync(bool forceReseed = false)
    {
        if (await _db.Set<Currency>().AnyAsync(c => c.Code == "USD"))
        {
            if (!forceReseed)
            {
                Console.WriteLine("Currency data already exists. Skipping seed.");
                return;
            }

            Console.WriteLine("Updating existing currency data...");
            await UpdateExistingCurrenciesAsync();
            return;
        }

        Console.WriteLine("Seeding currency data...");
        await SeedCurrenciesAsync();
        Console.WriteLine("Currency seed completed!");
    }

    private async Task SeedCurrenciesAsync()
    {
        var filePath = Path.Combine("SeedData", "currencies.json");
        if (!File.Exists(filePath))
        {
            Console.WriteLine($"  WARN: {filePath} not found. Skipping currencies.");
            return;
        }

        var json = await File.ReadAllTextAsync(filePath);
        var items = JsonSerializer.Deserialize<List<CurrencySeedItem>>(json, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        if (items == null || items.Count == 0)
        {
            Console.WriteLine("  WARN: No currencies found in JSON.");
            return;
        }

        var now = DateTime.UtcNow;
        var currencies = items.Select(i => new Currency
        {
            Id = i.Id,
            Code = i.Code,
            Symbol = i.Symbol,
            Name = i.Name,
            ExchangeRateToUSD = i.ExchangeRateToUSD,
            IsActive = i.IsActive,
            LastUpdated = now,
            CreatedAt = now,
            IsDeleted = false
        }).ToList();

        await _db.Set<Currency>().AddRangeAsync(currencies);
        await _db.SaveChangesAsync();
        Console.WriteLine($"  OK: {currencies.Count} para birimi eklendi.");
    }

    private async Task UpdateExistingCurrenciesAsync()
    {
        var filePath = Path.Combine("SeedData", "currencies.json");
        if (!File.Exists(filePath))
        {
            Console.WriteLine($"  WARN: {filePath} not found. Skipping update.");
            return;
        }

        var json = await File.ReadAllTextAsync(filePath);
        var items = JsonSerializer.Deserialize<List<CurrencySeedItem>>(json, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        if (items == null || items.Count == 0) return;

        var existing = await _db.Set<Currency>().ToListAsync();
        var map = existing.ToDictionary(c => c.Code, StringComparer.OrdinalIgnoreCase);
        var now = DateTime.UtcNow;
        var updated = 0;

        foreach (var item in items)
        {
            if (map.TryGetValue(item.Code, out var currency))
            {
                currency.Symbol = item.Symbol;
                currency.Name = item.Name;
                currency.IsActive = item.IsActive;
                currency.UpdatedAt = now;
                updated++;
            }
            else
            {
                // Yeni para birimi ekle
                await _db.Set<Currency>().AddAsync(new Currency
                {
                    Id = item.Id,
                    Code = item.Code,
                    Symbol = item.Symbol,
                    Name = item.Name,
                    ExchangeRateToUSD = item.ExchangeRateToUSD,
                    IsActive = item.IsActive,
                    LastUpdated = now,
                    CreatedAt = now,
                    IsDeleted = false
                });
                updated++;
            }
        }

        await _db.SaveChangesAsync();
        Console.WriteLine($"  OK: {updated} para birimi güncellendi/eklendi.");
    }

    private class CurrencySeedItem
    {
        public Guid Id { get; set; }
        public string Code { get; set; }
        public string Symbol { get; set; }
        public string Name { get; set; }
        public double ExchangeRateToUSD { get; set; }
        public bool IsActive { get; set; }
    }
}
