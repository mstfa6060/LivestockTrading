using LivestockTrading.Catalog.Domain.Aggregates;

namespace LivestockTrading.Catalog.Application.Abstractions;

/// <summary>
/// Thin repository port for Category aggregate root.
/// Implementation in Catalog.Infrastructure (Wave 3) wires EF Core.
/// SaveChanges semantics handled by IUnitOfWork pipeline filter, NOT this port.
/// </summary>
public interface ICategoryRepository
{
    Task<Category?> GetByIdAsync(int id, CancellationToken ct);
    Task<Category?> GetByCodeAsync(string code, CancellationToken ct);
    Task AddAsync(Category entity, CancellationToken ct);
    void Remove(Category entity);
}
