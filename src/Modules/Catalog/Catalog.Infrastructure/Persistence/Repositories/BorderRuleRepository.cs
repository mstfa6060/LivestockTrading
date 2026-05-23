using LivestockTrading.Catalog.Application.Abstractions;
using LivestockTrading.Catalog.Domain.Aggregates;

namespace LivestockTrading.Catalog.Infrastructure.Persistence.Repositories;

/// <summary>
/// IBorderRuleRepository impl. Kural 5:364 thin per-AR. Guid PK (BorderRule.cs:50 v7).
/// Brand paralel — GetByCode YOK. Port imzası (IBorderRuleRepository.cs:12-14) birebir.
/// </summary>
public sealed class BorderRuleRepository : IBorderRuleRepository
{
    private readonly CatalogDbContext _db;

    public BorderRuleRepository(CatalogDbContext db) => _db = db;

    public Task<BorderRule?> GetByIdAsync(Guid id, CancellationToken ct)
        => _db.Set<BorderRule>().FindAsync(new object[] { id }, ct).AsTask();

    public Task AddAsync(BorderRule entity, CancellationToken ct)
    {
        _db.Set<BorderRule>().Add(entity);
        return Task.CompletedTask;
    }

    public void Remove(BorderRule entity) => _db.Set<BorderRule>().Remove(entity);
}
