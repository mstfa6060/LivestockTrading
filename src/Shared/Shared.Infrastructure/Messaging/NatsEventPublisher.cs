using NATS.Client.Core;
using Shared.Contracts.Integration;

namespace Shared.Infrastructure.Messaging;

public sealed class NatsEventPublisher(INatsClient nats) : IEventPublisher
{
    public async Task PublishAsync<T>(string subject, T @event, CancellationToken ct = default)
        where T : IIntegrationEvent
    {
        await nats.PublishAsync(subject, @event, cancellationToken: ct);
    }
}
