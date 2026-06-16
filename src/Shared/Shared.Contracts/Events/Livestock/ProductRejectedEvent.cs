using Shared.Contracts.Integration;

namespace Shared.Contracts.Events.Livestock;

public record ProductRejectedEvent : IIntegrationEvent
{
    public const string Subject = "livestock.product.rejected";

    public Guid EventId { get; init; } = Guid.NewGuid();
    public DateTime OccurredAt { get; init; } = DateTime.UtcNow;
    public Guid ProductId { get; init; }
    public Guid SellerId { get; init; }
    public string? Reason { get; init; }
}
