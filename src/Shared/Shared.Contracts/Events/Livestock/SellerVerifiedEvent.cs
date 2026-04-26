using Shared.Contracts.Integration;

namespace Shared.Contracts.Events.Livestock;

public record SellerVerifiedEvent : IIntegrationEvent
{
    public const string Subject = "livestock.seller.verified";

    public Guid EventId { get; init; } = Guid.NewGuid();
    public DateTime OccurredAt { get; init; } = DateTime.UtcNow;
    public Guid SellerId { get; init; }
    public Guid UserId { get; init; }
}
