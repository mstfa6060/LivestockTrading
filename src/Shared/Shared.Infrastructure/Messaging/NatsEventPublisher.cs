using Microsoft.Extensions.Logging;
using NATS.Client.Core;
using Shared.Contracts.Integration;

namespace Shared.Infrastructure.Messaging;

public sealed class NatsEventPublisher(INatsClient nats, ILogger<NatsEventPublisher> logger) : IEventPublisher
{
    public async Task PublishAsync<T>(string subject, T @event, CancellationToken ct = default)
        where T : IIntegrationEvent
    {
        try
        {
            await nats.PublishAsync(subject, @event, cancellationToken: ct);
        }
        catch (Exception ex)
        {
            // Don't fail the originating request when the broker hiccups.
            // Persistence has already committed; integration consumers can be
            // backfilled from the audit/event tables (when those are wired up).
            logger.LogError(ex,
                "Failed to publish integration event to {Subject} ({EventType})",
                subject, typeof(T).Name);
        }
    }
}
