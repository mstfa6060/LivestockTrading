using LivestockTrading.Catalog.Application.Abstractions;

namespace LivestockTrading.Catalog.Infrastructure.Persistence.Repositories;

/// <summary>
/// IUnitOfWork impl — CatalogDbContext.SaveChangesAsync delegate.
/// Port (IUnitOfWork.cs:11) dönüş tipi `Task` (EF Core'un `Task&lt;int&gt;` dönüşü implicit
/// discard). Domain event dispatch W3.4'te SaveChanges interceptor üzerinden gelir;
/// UoW kendisi yalnız delege, DI registration'da interceptor wire edilir.
/// UnitOfWorkFilter.cs:12 grounded (Wave 3 wires IUnitOfWork to DbContext.SaveChangesAsync).
/// </summary>
public sealed class UnitOfWork : IUnitOfWork
{
    private readonly CatalogDbContext _db;

    public UnitOfWork(CatalogDbContext db) => _db = db;

    public Task SaveChangesAsync(CancellationToken ct) => _db.SaveChangesAsync(ct);
}
