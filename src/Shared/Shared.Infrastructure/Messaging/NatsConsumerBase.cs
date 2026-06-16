using Microsoft.Extensions.Hosting;
using NATS.Client.Core;
using Shared.Contracts.Integration;

namespace Shared.Infrastructure.Messaging;

/// <summary>
/// Base class for NATS JetStream consumers. PR#6 Workers extend this.
/// </summary>
public abstract class NatsConsumerBase<T>(INatsClient nats) : BackgroundService
    where T : IIntegrationEvent
{
    protected abstract string Subject { get; }

    protected abstract Task HandleAsync(T message, CancellationToken ct);

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await foreach (var msg in nats.SubscribeAsync<T>(Subject, cancellationToken: stoppingToken))
        {
            if (msg.Data is not null)
            {
                await HandleAsync(msg.Data, stoppingToken);
            }
        }
    }
}
