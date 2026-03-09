using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using LivestockTrading.Domain.Entities;

namespace MigrationJob.SeedData;

/// <summary>
/// Kategori seed verilerini veritabanına yükler
/// 12 ana kategori + 58 alt kategori, 15 dilde çeviri
/// </summary>
public class CategorySeeder
{
    private readonly DbContext _db;
    private readonly JsonSerializerOptions _jsonOptions;

    public CategorySeeder(DbContext db)
    {
        _db = db;
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };
    }

    public async Task SeedAsync(bool forceReseed = false)
    {
        if (await _db.Set<Category>().AnyAsync())
        {
            if (!forceReseed)
            {
                Console.WriteLine("Category data already exists. Skipping seed.");
                return;
            }

            Console.WriteLine("Updating existing category data...");
            await UpdateExistingCategoriesAsync();
            return;
        }

        Console.WriteLine("Seeding category data...");
        await SeedCategoriesAsync();
        Console.WriteLine("Category seed completed!");
    }

    private async Task UpdateExistingCategoriesAsync()
    {
        var filePath = Path.Combine("SeedData", "categories.json");
        if (!File.Exists(filePath))
        {
            Console.WriteLine($"  WARN: {filePath} not found. Skipping update.");
            return;
        }

        var json = await File.ReadAllTextAsync(filePath);
        var categories = JsonSerializer.Deserialize<List<CategorySeedModel>>(json, _jsonOptions);

        if (categories == null || categories.Count == 0)
        {
            Console.WriteLine("  WARN: No categories found in JSON.");
            return;
        }

        var existingCategories = await _db.Set<Category>().ToListAsync();
        var updated = 0;
        var added = 0;

        // Önce root kategorileri ekle/güncelle, sonra alt kategorileri
        var rootCategories = categories.Where(c => c.ParentCategoryId == null).ToList();
        var subCategories = categories.Where(c => c.ParentCategoryId != null).ToList();

        foreach (var source in rootCategories.Concat(subCategories))
        {
            var existing = existingCategories.FirstOrDefault(c => c.Id == source.Id);
            if (existing != null)
            {
                existing.Name = source.Name;
                existing.Slug = source.Slug;
                existing.Description = source.Description;
                existing.SortOrder = source.SortOrder;
                existing.IsActive = source.IsActive;
                existing.ParentCategoryId = source.ParentCategoryId;
                existing.NameTranslations = source.NameTranslations;
                existing.DescriptionTranslations = source.DescriptionTranslations;
                existing.IconUrl = source.IconUrl;
                existing.AttributesTemplate = source.AttributesTemplate;
                updated++;
            }
            else
            {
                var newCategory = new Category
                {
                    Id = source.Id,
                    Name = source.Name,
                    Slug = source.Slug,
                    Description = source.Description,
                    SortOrder = source.SortOrder,
                    IsActive = source.IsActive,
                    ParentCategoryId = source.ParentCategoryId,
                    NameTranslations = source.NameTranslations,
                    DescriptionTranslations = source.DescriptionTranslations,
                    IconUrl = source.IconUrl,
                    AttributesTemplate = source.AttributesTemplate
                };
                await _db.Set<Category>().AddAsync(newCategory);
                added++;
            }
        }

        await _db.SaveChangesAsync();
        Console.WriteLine($"  OK: {updated} kategori guncellendi, {added} yeni kategori eklendi.");
    }

    private async Task SeedCategoriesAsync()
    {
        var filePath = Path.Combine("SeedData", "categories.json");
        if (!File.Exists(filePath))
        {
            Console.WriteLine($"  WARN: {filePath} not found. Skipping categories.");
            return;
        }

        var json = await File.ReadAllTextAsync(filePath);
        var seedModels = JsonSerializer.Deserialize<List<CategorySeedModel>>(json, _jsonOptions);

        if (seedModels == null || seedModels.Count == 0)
        {
            Console.WriteLine("  WARN: No categories found in JSON.");
            return;
        }

        // Root kategorileri önce ekle (FK constraint)
        var rootCategories = seedModels.Where(c => c.ParentCategoryId == null)
            .Select(MapToEntity).ToList();
        var subCategories = seedModels.Where(c => c.ParentCategoryId != null)
            .Select(MapToEntity).ToList();

        await _db.Set<Category>().AddRangeAsync(rootCategories);
        await _db.SaveChangesAsync();

        await _db.Set<Category>().AddRangeAsync(subCategories);
        await _db.SaveChangesAsync();

        Console.WriteLine($"  OK: {seedModels.Count} kategori eklendi ({rootCategories.Count} ana, {subCategories.Count} alt).");
    }

    private static Category MapToEntity(CategorySeedModel model)
    {
        return new Category
        {
            Id = model.Id,
            Name = model.Name,
            Slug = model.Slug,
            Description = model.Description,
            SortOrder = model.SortOrder,
            IsActive = model.IsActive,
            ParentCategoryId = model.ParentCategoryId,
            NameTranslations = model.NameTranslations,
            DescriptionTranslations = model.DescriptionTranslations,
            IconUrl = model.IconUrl,
            AttributesTemplate = model.AttributesTemplate
        };
    }
}

/// <summary>
/// JSON deserializasyon modeli - Category entity'den bağımsız
/// </summary>
public class CategorySeedModel
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Slug { get; set; }
    public string Description { get; set; }
    public int SortOrder { get; set; }
    public bool IsActive { get; set; }
    public Guid? ParentCategoryId { get; set; }
    public string NameTranslations { get; set; }
    public string DescriptionTranslations { get; set; }
    public string IconUrl { get; set; }
    public string AttributesTemplate { get; set; }
}
