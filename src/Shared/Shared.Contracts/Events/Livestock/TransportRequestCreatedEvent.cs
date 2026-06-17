using Shared.Contracts.Integration;

namespace Shared.Contracts.Events.Livestock;

public record TransportRequestCreatedEvent : IIntegrationEvent
{
    public const string Subject = "livestock.transport-request.created";

    public Guid EventId { get; init; } = Guid.NewGuid();
    public DateTime OccurredAt { get; init; } = DateTime.UtcNow;
    public Guid TransportRequestId { get; init; }
    public Guid RequesterUserId { get; init; }
    public string PickupCountryCode { get; init; } = string.Empty;
    public string PickupCity { get; init; } = string.Empty;
    public string DeliveryCountryCode { get; init; } = string.Empty;
    public string DeliveryCity { get; init; } = string.Empty;
}
