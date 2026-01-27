using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Common.Definitions.Domain.Entities;

namespace MigrationJob.SeedData;

/// <summary>
/// Rol seed verilerini veritabanına yükler
/// Platform için tanımlanmış 7 sistem rolü
/// </summary>
public class RoleSeeder
{
    private readonly DbContext _db;
    private readonly JsonSerializerOptions _jsonOptions;

    public RoleSeeder(DbContext db)
    {
        _db = db;
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };
    }

    public async Task SeedAsync(bool forceReseed = false)
    {
        if (await _db.Set<Role>().AnyAsync())
        {
            if (!forceReseed)
            {
                Console.WriteLine("Role data already exists. Skipping seed.");
                return;
            }

            // Force reseed - mevcut verileri güncelle
            Console.WriteLine("Updating existing role data...");
            await UpdateExistingRolesAsync();
            return;
        }

        Console.WriteLine("Seeding role data...");
        await SeedRolesAsync();
        Console.WriteLine("Role seed completed!");
    }

    private async Task UpdateExistingRolesAsync()
    {
        var filePath = Path.Combine("SeedData", "roles.json");
        if (!File.Exists(filePath))
        {
            Console.WriteLine($"  WARN: {filePath} not found. Skipping update.");
            return;
        }

        var json = await File.ReadAllTextAsync(filePath);
        var roles = JsonSerializer.Deserialize<List<Role>>(json, _jsonOptions);

        if (roles == null || roles.Count == 0)
        {
            Console.WriteLine("  WARN: No roles found in JSON.");
            return;
        }

        var existingRoles = await _db.Set<Role>().ToListAsync();
        var updated = 0;
        var added = 0;

        foreach (var source in roles)
        {
            var existing = existingRoles.FirstOrDefault(r => r.Id == source.Id);
            if (existing != null)
            {
                existing.Name = source.Name;
                existing.IsSystemRole = source.IsSystemRole;
                updated++;
            }
            else
            {
                // Yeni rol ekle
                await _db.Set<Role>().AddAsync(source);
                added++;
            }
        }

        await _db.SaveChangesAsync();
        Console.WriteLine($"  OK: {updated} rol guncellendi, {added} yeni rol eklendi.");
    }

    private async Task SeedRolesAsync()
    {
        var filePath = Path.Combine("SeedData", "roles.json");
        if (!File.Exists(filePath))
        {
            Console.WriteLine($"  WARN: {filePath} not found. Skipping roles.");
            return;
        }

        var json = await File.ReadAllTextAsync(filePath);
        var roles = JsonSerializer.Deserialize<List<Role>>(json, _jsonOptions);

        if (roles == null || roles.Count == 0)
        {
            Console.WriteLine("  WARN: No roles found in JSON.");
            return;
        }

        await _db.Set<Role>().AddRangeAsync(roles);
        await _db.SaveChangesAsync();
        Console.WriteLine($"  OK: {roles.Count} rol eklendi.");
    }
}
