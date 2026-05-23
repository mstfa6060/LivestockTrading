using LivestockTrading.Catalog.Application.Abstractions;
using LivestockTrading.Catalog.Domain.Aggregates;

namespace LivestockTrading.Catalog.Infrastructure.Persistence.Repositories;

/// <summary>
/// IBrandRepository impl. Kural 5:364 thin per-AR. Guid PK (Brand.cs:68 v7 app-assigned).
/// GetByCode/GetBySlug port'ta YOK (read W3.5 EF projection, KAYDET-14 grounded).
/// Port imzası (IBrandRepository.cs:12-14) birebir — W1-4 port baskın.
/// </summary>
public sealed class BrandRepository : IBrandRepository
{
    private readonly CatalogDbContext _db;

    public BrandRepository(CatalogDbContext db) => _db = db;

    public Task<Brand?> GetByIdAsync(Guid id, CancellationToken ct)
        => _db.Set<Brand>().FindAsync(new object[] { id }, ct).AsTask();

    public Task AddAsync(Brand entity, CancellationToken ct)
    {
        _db.Set<Brand>().Add(entity);
        return Task.CompletedTask;
    }

    public void Remove(Brand entity) => _db.Set<Brand>().Remove(entity);
}
