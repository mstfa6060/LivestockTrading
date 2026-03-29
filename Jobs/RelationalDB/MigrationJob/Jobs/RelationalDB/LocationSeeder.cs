using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Common.Definitions.Domain.Entities;

namespace MigrationJob.SeedData;

/// <summary>
/// GeoNames verilerinden Province, District ve Neighborhood seed verilerini yükler.
/// provinces.json, districts.json ve neighborhoods.json dosyalarını okur.
/// neighborhoods.json 640MB+ olabilir — streaming JSON reader ile okunur.
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
        await SeedNeighborhoodsAsync();
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

        await SeedBatchAsync(_db.Set<Province>(), provinces, "il/eyalet");
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

        await SeedBatchAsync(_db.Set<District>(), districts, "ilçe/şehir");
    }

    private async Task SeedNeighborhoodsAsync()
    {
        var filePath = Path.Combine("SeedData", "neighborhoods.json");
        if (!File.Exists(filePath))
        {
            Console.WriteLine($"  INFO: {filePath} not found. Skipping neighborhoods.");
            return;
        }

        Console.WriteLine("  Neighborhoods seed başlıyor (streaming)...");

        var strategy = _db.Database.CreateExecutionStrategy();
        await strategy.ExecuteAsync(async () =>
        {
            const int batchSize = 1000; // Büyük veri için daha büyük batch
            var batch = new List<Neighborhood>(batchSize);
            var totalCount = 0;
            var batchCount = 0;

            using var transaction = await _db.Database.BeginTransactionAsync();
            try
            {
                await using var stream = File.OpenRead(filePath);
                var neighborhoods = JsonSerializer.DeserializeAsyncEnumerable<Neighborhood>(stream, _jsonOptions);

                await foreach (var n in neighborhoods)
                {
                    if (n == null) continue;
                    batch.Add(n);
                    totalCount++;

                    if (batch.Count >= batchSize)
                    {
                        await _db.Set<Neighborhood>().AddRangeAsync(batch);
                        await _db.SaveChangesAsync();
                        _db.ChangeTracker.Clear();
                        batchCount++;
                        batch.Clear();

                        if (batchCount % 100 == 0)
                            Console.Write($"\r  {totalCount:N0} mahalle eklendi ({batchCount} batch)...");
                    }
                }

                // Kalan batch
                if (batch.Count > 0)
                {
                    await _db.Set<Neighborhood>().AddRangeAsync(batch);
                    await _db.SaveChangesAsync();
                    _db.ChangeTracker.Clear();
                    batchCount++;
                }

                await transaction.CommitAsync();
                Console.WriteLine($"\r  OK: {totalCount:N0} mahalle eklendi ({batchCount} batch).");
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        });
    }

    /// <summary>
    /// Genel batch seed helper (Province, District gibi küçük veri setleri için)
    /// </summary>
    private async Task SeedBatchAsync<T>(DbSet<T> dbSet, List<T> items, string label) where T : class
    {
        var strategy = _db.Database.CreateExecutionStrategy();
        await strategy.ExecuteAsync(async () =>
        {
            const int batchSize = 500;
            var batches = items
                .Select((item, index) => new { item, index })
                .GroupBy(x => x.index / batchSize)
                .Select(g => g.Select(x => x.item).ToList())
                .ToList();

            using var transaction = await _db.Database.BeginTransactionAsync();
            try
            {
                foreach (var batch in batches)
                {
                    await dbSet.AddRangeAsync(batch);
                    await _db.SaveChangesAsync();
                    _db.ChangeTracker.Clear();
                }

                await transaction.CommitAsync();
                Console.WriteLine($"  OK: {items.Count:N0} {label} eklendi ({batches.Count} batch).");
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        });
    }
}
