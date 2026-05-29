namespace LivestockTrading.Identity.Application.Abstractions;

/// <summary>
/// Unit of work abstraction — SaveChanges semantics for Identity module.
/// Implementation in Identity.Infrastructure (Wave 4 W4.3) wires EF Core DbContext.
/// Pipeline filter (UnitOfWorkFilter) invokes after handler completion to
/// commit aggregate mutations within a single transaction.
/// </summary>
public interface IUnitOfWork
{
    Task SaveChangesAsync(CancellationToken ct);
}
