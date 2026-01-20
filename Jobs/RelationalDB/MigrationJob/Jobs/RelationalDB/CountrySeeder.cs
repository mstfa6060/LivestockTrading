using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Common.Definitions.Domain.Entities;

namespace MigrationJob.SeedData;

/// <summary>
/// Ülke seed verilerini veritabanına yükler
/// ISO 3166-1 standardına uygun 196 ülke
/// </summary>
public class CountrySeeder
{
    private readonly DbContext _db;
    private readonly JsonSerializerOptions _jsonOptions;

    public CountrySeeder(DbContext db)
    {
        _db = db;
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };
    }

    public async Task SeedAsync(bool forceReseed = false)
    {
        // Eğer veri varsa ve force değilse seed yapma
        if (await _db.Set<Country>().AnyAsync())
        {
            if (!forceReseed)
            {
                Console.WriteLine("Country data already exists. Skipping seed.");
                return;
            }

            // Force reseed - mevcut verileri sil
            Console.WriteLine("Deleting existing country data...");
            await DeleteExistingDataAsync();
        }

        Console.WriteLine("Seeding country data...");
        await SeedCountriesAsync();
        Console.WriteLine("Country seed completed!");
    }

    private async Task DeleteExistingDataAsync()
    {
        // Önce FK referanslarını kontrol et ve NULL yap
        Console.WriteLine("  -> FK referanslari temizleniyor...");

        var cleanupQueries = new[]
        {
            // User tablosundaki CountryId referansını NULL yap
            "UPDATE AppUsers SET CountryId = NULL WHERE CountryId IS NOT NULL"
        };

        foreach (var query in cleanupQueries)
        {
            try
            {
                await _db.Database.ExecuteSqlRawAsync(query);
                Console.WriteLine($"  OK: {query.Substring(0, Math.Min(60, query.Length))}...");
            }
            catch (Exception ex)
            {
                // Tablo yoksa veya kolon yoksa devam et
                Console.WriteLine($"  Skipped: {ex.Message.Split('\n')[0]}");
            }
        }

        // Country verilerini sil
        var countryCount = await _db.Set<Country>().CountAsync();
        if (countryCount > 0)
        {
            await _db.Set<Country>().ExecuteDeleteAsync();
            Console.WriteLine($"  OK: {countryCount:N0} ulke silindi.");
        }
    }

    private async Task SeedCountriesAsync()
    {
        var filePath = Path.Combine("SeedData", "countries.json");
        if (!File.Exists(filePath))
        {
            Console.WriteLine($"  WARN: {filePath} not found. Skipping countries.");
            return;
        }

        var json = await File.ReadAllTextAsync(filePath);
        var countries = JsonSerializer.Deserialize<List<Country>>(json, _jsonOptions);

        if (countries == null || countries.Count == 0)
        {
            Console.WriteLine("  WARN: No countries found in JSON.");
            return;
        }

        // IDENTITY_INSERT acik olmali cunku Id degerlerini manuel veriyoruz
        await _db.Database.ExecuteSqlRawAsync("SET IDENTITY_INSERT [Countries] ON");

        try
        {
            await _db.Set<Country>().AddRangeAsync(countries);
            await _db.SaveChangesAsync();
            Console.WriteLine($"  OK: {countries.Count} ulke eklendi.");
        }
        finally
        {
            await _db.Database.ExecuteSqlRawAsync("SET IDENTITY_INSERT [Countries] OFF");
        }
    }
}
