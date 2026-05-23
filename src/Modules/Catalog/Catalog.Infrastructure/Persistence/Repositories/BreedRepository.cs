using LivestockTrading.Catalog.Application.Abstractions;
using LivestockTrading.Catalog.Domain.Aggregates;
using Microsoft.EntityFrameworkCore;

namespace LivestockTrading.Catalog.Infrastructure.Persistence.Repositories;

/// <summary>
/// IBreedRepository impl. Kural 5:364 thin per-AR; GetByCode command-side natural-key
/// resolve (port'ta var). Category paralel. Port imzası (IBreedRepository.cs:12-15)
/// birebir — W1-4 port baskın.
/// </summary>
public sealed class BreedRepository : IBreedRepository
{
    private readonly CatalogDbContext _db;

    public BreedRepository(CatalogDbContext db) => _db = db;

    public Task<Breed?> GetByIdAsync(int id, CancellationToken ct)
        => _db.Set<Breed>().FindAsync(new object[] { id }, ct).AsTask();

    public Task<Breed?> GetByCodeAsync(string code, CancellationToken ct)
        => _db.Set<Breed>().FirstOrDefaultAsync(b => b.Code == code, ct);

    public Task AddAsync(Breed entity, CancellationToken ct)
    {
        _db.Set<Breed>().Add(entity);
        return Task.CompletedTask;
    }

    public void Remove(Breed entity) => _db.Set<Breed>().Remove(entity);
}
