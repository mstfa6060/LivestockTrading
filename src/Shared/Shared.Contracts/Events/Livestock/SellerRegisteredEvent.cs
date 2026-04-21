using Shared.Contracts.Integration;

namespace Shared.Contracts.Events.Livestock;

public record SellerRegisteredEvent : IIntegrationEvent
{
    public const string Subject = "livestock.seller.registered";

    public Guid EventId { get; init; } = Guid.NewGuid();
    public DateTime OccurredAt { get; init; } = DateTime.UtcNow;
    public Guid SellerId { get; init; }
    public Guid UserId { get; init; }
    public string BusinessName { get; init; } = string.Empty;
}
