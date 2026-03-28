using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Common.Definitions.Domain.Entities;

namespace MigrationJob.SeedData;

/// <summary>
/// GeoNames verilerinden Province ve District seed verilerini yükler.
/// provinces.json ve districts.json dosyalarını okur.
/// </summary>
public class LocationSeeder
{
    private readonly DbContext _db;
    private readonly JsonSerializerOptions _jsonOptions;

    public LocationSeeder(DbContext db)
    {
        _db = db;
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };
    }

    public async Task SeedAsync(bool forceReseed = false)
    {
        var hasProvinces = await _db.Set<Province>().AnyAsync();

        if (hasProvinces)
        {
            if (!forceReseed)
            {
                Console.WriteLine("Location data already exists. Skipping seed.");
                return;
            }

            Console.WriteLine("Force reseed: Deleting existing location data...");
            await DeleteExistingDataAsync();
        }

        Console.WriteLine("Seeding location data from GeoNames...");
        await SeedProvincesAsync();
        await SeedDistrictsAsync();
        Console.WriteLine("Location seed completed!");
    }

    private async Task DeleteExistingDataAsync()
    {
        // Locations tablosundaki DistrictId referanslarını temizle (FK constraint)
        await _db.Database.ExecuteSqlRawAsync("UPDATE Locations SET DistrictId = NULL WHERE DistrictId IS NOT NULL");
        Console.WriteLine("  OK: Locations.DistrictId referansları temizlendi.");

        // FK sırasına dikkat: önce Neighborhoods, sonra Districts, sonra Provinces
        var neighborhoodCount = await _db.Set<Neighborhood>().CountAsync();
        if (neighborhoodCount > 0)
        {
            await _db.Set<Neighborhood>().ExecuteDeleteAsync();
            Console.WriteLine($"  OK: {neighborhoodCount:N0} mahalle silindi.");
        }

        var districtCount = await _db.Set<District>().CountAsync();
        if (districtCount > 0)
        {
            await _db.Set<District>().ExecuteDeleteAsync();
            Console.WriteLine($"  OK: {districtCount:N0} ilçe/şehir silindi.");
        }

        var provinceCount = await _db.Set<Province>().CountAsync();
        if (provinceCount > 0)
        {
            await _db.Set<Province>().ExecuteDeleteAsync();
            Console.WriteLine($"  OK: {provinceCount:N0} il/eyalet silindi.");
        }
    }

    private async Task SeedProvincesAsync()
    {
        var filePath = Path.Combine("SeedData", "provinces.json");
        if (!File.Exists(filePath))
        {
            Console.WriteLine($"  WARN: {filePath} not found. Skipping provinces.");
            return;
        }

        var json = await File.ReadAllTextAsync(filePath);
        var provinces = JsonSerializer.Deserialize<List<Province>>(json, _jsonOptions);

        if (provinces == null || provinces.Count == 0)
        {
            Console.WriteLine("  WARN: No provinces found in JSON.");
            return;
        }

        var strategy = _db.Database.CreateExecutionStrategy();
        await strategy.ExecuteAsync(async () =>
        {
            // Büyük veri setini batch'ler halinde ekle (SQL Server parametre limiti)
            const int batchSize = 500;
            var batches = provinces
                .Select((item, index) => new { item, index })
                .GroupBy(x => x.index / batchSize)
                .Select(g => g.Select(x => x.item).ToList())
                .ToList();

            using var transaction = await _db.Database.BeginTransactionAsync();
            try
            {
                // Province.Id is ValueGeneratedNever — no IDENTITY_INSERT needed

                foreach (var batch in batches)
                {
                    await _db.Set<Province>().AddRangeAsync(batch);
                    await _db.SaveChangesAsync();
                    _db.ChangeTracker.Clear(); // Memory'yi temizle
                }

                // No IDENTITY_INSERT OFF needed

                await transaction.CommitAsync();
                Console.WriteLine($"  OK: {provinces.Count} il/eyalet eklendi ({batches.Count} batch).");
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        });
    }

    private async Task SeedDistrictsAsync()
    {
        var filePath = Path.Combine("SeedData", "districts.json");
        if (!File.Exists(filePath))
        {
            Console.WriteLine($"  WARN: {filePath} not found. Skipping districts.");
            return;
        }

        var json = await File.ReadAllTextAsync(filePath);
        var districts = JsonSerializer.Deserialize<List<District>>(json, _jsonOptions);

        if (districts == null || districts.Count == 0)
        {
            Console.WriteLine("  WARN: No districts found in JSON.");
            return;
        }

        var strategy = _db.Database.CreateExecutionStrategy();
        await strategy.ExecuteAsync(async () =>
        {
            const int batchSize = 500;
            var batches = districts
                .Select((item, index) => new { item, index })
                .GroupBy(x => x.index / batchSize)
                .Select(g => g.Select(x => x.item).ToList())
                .ToList();

            using var transaction = await _db.Database.BeginTransactionAsync();
            try
            {
                // District.Id is ValueGeneratedNever — no IDENTITY_INSERT needed

                foreach (var batch in batches)
                {
                    await _db.Set<District>().AddRangeAsync(batch);
                    await _db.SaveChangesAsync();
                    _db.ChangeTracker.Clear();
                }

                // No IDENTITY_INSERT OFF needed

                await transaction.CommitAsync();
                Console.WriteLine($"  OK: {districts.Count} ilçe/şehir eklendi ({batches.Count} batch).");
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        });
    }
}
