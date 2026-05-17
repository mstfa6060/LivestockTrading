using LivestockTrading.Catalog.Domain.Aggregates;

namespace LivestockTrading.Catalog.Application.Abstractions;

/// <summary>
/// Thin repository port for Brand aggregate root.
/// Implementation in Catalog.Infrastructure (Wave 3) wires EF Core.
/// SaveChanges semantics handled by IUnitOfWork pipeline filter, NOT this port.
/// </summary>
public interface IBrandRepository
{
    Task<Brand?> GetByIdAsync(Guid id, CancellationToken ct);
    Task AddAsync(Brand entity, CancellationToken ct);
    void Remove(Brand entity);
}
