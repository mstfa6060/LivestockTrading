namespace LivestockTrading.Catalog.Application.Abstractions;

/// <summary>
/// Unit of work abstraction — SaveChanges semantics for Catalog module.
/// Implementation in Catalog.Infrastructure (Wave 3) wires EF Core DbContext.
/// Pipeline filter (UnitOfWorkFilter) invokes after handler completion to
/// commit aggregate mutations within a single transaction.
/// </summary>
public interface IUnitOfWork
{
    Task SaveChangesAsync(CancellationToken ct);
}
