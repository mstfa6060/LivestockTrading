using Shared.Contracts.Integration;

namespace Shared.Infrastructure.Messaging;

/// No-op publisher used by NSwag/EF design-time scenarios where the host is
/// booted purely for metadata discovery and no real broker is reachable.
public sealed class NoopEventPublisher : IEventPublisher
{
    public Task PublishAsync<T>(string subject, T @event, CancellationToken ct = default)
        where T : IIntegrationEvent
        => Task.CompletedTask;
}
