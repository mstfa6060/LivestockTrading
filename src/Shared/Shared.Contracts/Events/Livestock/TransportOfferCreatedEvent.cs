using Shared.Contracts.Integration;

namespace Shared.Contracts.Events.Livestock;

public record TransportOfferCreatedEvent : IIntegrationEvent
{
    public const string Subject = "livestock.transport-offer.created";

    public Guid EventId { get; init; } = Guid.NewGuid();
    public DateTime OccurredAt { get; init; } = DateTime.UtcNow;
    public Guid TransportOfferId { get; init; }
    public Guid TransportRequestId { get; init; }
    public Guid TransporterId { get; init; }
    public Guid RequesterUserId { get; init; }
    public decimal Price { get; init; }
    public string CurrencyCode { get; init; } = string.Empty;
}
