using Shared.Contracts.Integration;

namespace Shared.Contracts.Events.Livestock;

public record OfferAcceptedEvent : IIntegrationEvent
{
    public const string Subject = "livestock.offer.accepted";

    public Guid EventId { get; init; } = Guid.NewGuid();
    public DateTime OccurredAt { get; init; } = DateTime.UtcNow;
    public Guid OfferId { get; init; }
    public Guid DealId { get; init; }
    public Guid ProductId { get; init; }
    public Guid BuyerUserId { get; init; }
    public Guid SellerId { get; init; }
    public decimal AgreePrice { get; init; }
    public string CurrencyCode { get; init; } = string.Empty;
}
