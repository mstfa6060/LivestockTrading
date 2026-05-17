using LivestockTrading.Catalog.Domain.Aggregates;

namespace LivestockTrading.Catalog.Application.Abstractions;

/// <summary>
/// Thin repository port for Breed aggregate root.
/// Implementation in Catalog.Infrastructure (Wave 3) wires EF Core.
/// SaveChanges semantics handled by IUnitOfWork pipeline filter, NOT this port.
/// </summary>
public interface IBreedRepository
{
    Task<Breed?> GetByIdAsync(int id, CancellationToken ct);
    Task AddAsync(Breed entity, CancellationToken ct);
    void Remove(Breed entity);
}
