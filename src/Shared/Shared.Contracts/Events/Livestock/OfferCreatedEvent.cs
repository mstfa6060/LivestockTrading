using Shared.Contracts.Integration;

namespace Shared.Contracts.Events.Livestock;

public record OfferCreatedEvent : IIntegrationEvent
{
    public const string Subject = "livestock.offer.created";

    public Guid EventId { get; init; } = Guid.NewGuid();
    public DateTime OccurredAt { get; init; } = DateTime.UtcNow;
    public Guid OfferId { get; init; }
    public Guid ProductId { get; init; }
    public Guid BuyerUserId { get; init; }
    public Guid SellerId { get; init; }
    public decimal OfferedPrice { get; init; }
    public string CurrencyCode { get; init; } = string.Empty;
    public int Quantity { get; init; }
}
