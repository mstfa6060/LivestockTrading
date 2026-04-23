using Shared.Contracts.Integration;

namespace Shared.Contracts.Events.Livestock;

public record ProductApprovedForSocialMediaEvent : IIntegrationEvent
{
    public const string Subject = "livestock.product.approved.socialmedia";

    public Guid EventId { get; init; } = Guid.NewGuid();
    public DateTime OccurredAt { get; init; } = DateTime.UtcNow;
    public Guid ProductId { get; init; }
    public Guid SellerId { get; init; }
    public string Title { get; init; } = string.Empty;
    public string Slug { get; init; } = string.Empty;
    public decimal Price { get; init; }
    public string CurrencyCode { get; init; } = string.Empty;
}
