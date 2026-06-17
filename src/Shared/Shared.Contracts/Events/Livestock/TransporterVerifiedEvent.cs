using Shared.Contracts.Integration;

namespace Shared.Contracts.Events.Livestock;

public record TransporterVerifiedEvent : IIntegrationEvent
{
    public const string Subject = "livestock.transporter.verified";

    public Guid EventId { get; init; } = Guid.NewGuid();
    public DateTime OccurredAt { get; init; } = DateTime.UtcNow;
    public Guid TransporterId { get; init; }
    public Guid UserId { get; init; }
}
