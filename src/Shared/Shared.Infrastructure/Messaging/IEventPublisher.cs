using Shared.Contracts.Integration;

namespace Shared.Infrastructure.Messaging;

public interface IEventPublisher
{
    Task PublishAsync<T>(string subject, T @event, CancellationToken ct = default)
        where T : IIntegrationEvent;
}
