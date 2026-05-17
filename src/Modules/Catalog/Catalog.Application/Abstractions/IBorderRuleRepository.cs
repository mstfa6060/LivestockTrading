using LivestockTrading.Catalog.Domain.Aggregates;

namespace LivestockTrading.Catalog.Application.Abstractions;

/// <summary>
/// Thin repository port for BorderRule aggregate root.
/// Implementation in Catalog.Infrastructure (Wave 3) wires EF Core.
/// SaveChanges semantics handled by IUnitOfWork pipeline filter, NOT this port.
/// </summary>
public interface IBorderRuleRepository
{
    Task<BorderRule?> GetByIdAsync(Guid id, CancellationToken ct);
    Task AddAsync(BorderRule entity, CancellationToken ct);
    void Remove(BorderRule entity);
}
