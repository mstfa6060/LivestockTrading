using LivestockTrading.Catalog.Application.Abstractions;
using LivestockTrading.Catalog.Domain.Aggregates;
using Microsoft.EntityFrameworkCore;

namespace LivestockTrading.Catalog.Infrastructure.Persistence.Repositories;

/// <summary>
/// ICategoryRepository impl. 03-domain-patterns.md Kural 5:364 thin per-AR
/// (GetById/Add/Remove); GetByCode command-side natural-key resolve (port'ta var,
/// LINQ FirstOrDefaultAsync). Query/list method YOK (Kural 5:373 read W3.5 EF projection).
/// Port imzası (ICategoryRepository.cs:12-15) birebir — W1-4 port baskın.
/// </summary>
public sealed class CategoryRepository : ICategoryRepository
{
    private readonly CatalogDbContext _db;

    public CategoryRepository(CatalogDbContext db) => _db = db;

    public Task<Category?> GetByIdAsync(int id, CancellationToken ct)
        => _db.Set<Category>().FindAsync(new object[] { id }, ct).AsTask();

    public Task<Category?> GetByCodeAsync(string code, CancellationToken ct)
        => _db.Set<Category>().FirstOrDefaultAsync(c => c.Code == code, ct);

    public Task AddAsync(Category entity, CancellationToken ct)
    {
        _db.Set<Category>().Add(entity);   // EF Add senkron; port async imza Task.CompletedTask ile karşılanır
        return Task.CompletedTask;
    }

    public void Remove(Category entity) => _db.Set<Category>().Remove(entity);
}
