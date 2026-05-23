using MassTransit;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Logging;
using Shared.Domain;

namespace LivestockTrading.Catalog.Infrastructure.Persistence;

/// <summary>
/// EF Core SaveChangesInterceptor — post-commit AR.DomainEvents in-process publish.
/// Wave 1 AggregateRoot.cs:13 yorumu birebir: "Infrastructure UoW SaveChanges sonrası
/// dispatch + Clear eder". Plan-1 Karar 1.c (Catalog-local dispatcher) + Karar 3b
/// (in-process MassTransit.Mediator, doc-literal). Faz 1: outbox YOK (Wave 5+ at-least-once
/// garanti). Commit baskın, dispatch best-effort (post-commit hook, rollback imkânsız).
/// W3.7'de CatalogDbContext options'a `AddInterceptors(sp.GetRequiredService&lt;...&gt;())`
/// ile wire edilecek (KARAR 4 (b), DbContext registration W3.7 host-wire).
/// </summary>
public sealed class DomainEventDispatchInterceptor : SaveChangesInterceptor
{
    private readonly IPublishEndpoint _publishEndpoint;
    private readonly ILogger<DomainEventDispatchInterceptor> _logger;

    public DomainEventDispatchInterceptor(
        IPublishEndpoint publishEndpoint,
        ILogger<DomainEventDispatchInterceptor> logger)
    {
        _publishEndpoint = publishEndpoint;
        _logger = logger;
    }

    public override async ValueTask<int> SavedChangesAsync(
        SaveChangesCompletedEventData eventData,
        int result,
        CancellationToken ct = default)
    {
        if (eventData.Context is not CatalogDbContext db)
            return result;

        // Snapshot (Clear öncesi, in-process handler-induced Raise yeni event'leri bir
        // sonraki UoW turuna iter — DDD-doğru semantic, foreach mutation safety).
        var pending = db.ChangeTracker.Entries<AggregateRoot>()
            .Where(e => e.Entity.DomainEvents.Count > 0)
            .Select(e => (Aggregate: e.Entity, Events: e.Entity.DomainEvents.ToList()))
            .ToList();

        // Clear ilk (publish öncesi)
        foreach (var (aggregate, _) in pending)
            aggregate.ClearDomainEvents();

        // Publish (per-event try/catch swallow + log; commit zaten kapandı, transaction integrity baskın)
        foreach (var (_, events) in pending)
        {
            foreach (var domainEvent in events)
            {
                try
                {
                    await _publishEndpoint.Publish(domainEvent, ct);
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex,
                        "Domain event dispatch failed (type={EventType}); commit korundu, event kaybı (Faz 1, outbox W5+).",
                        domainEvent.GetType().Name);
                }
            }
        }

        return result;
    }
}
