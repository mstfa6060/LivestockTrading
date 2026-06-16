using Shared.Contracts.Integration;

namespace Shared.Contracts.Events.Livestock;

public record TransporterRegisteredEvent : IIntegrationEvent
{
    public const string Subject = "livestock.transporter.registered";

    public Guid EventId { get; init; } = Guid.NewGuid();
    public DateTime OccurredAt { get; init; } = DateTime.UtcNow;
    public Guid TransporterId { get; init; }
    public Guid UserId { get; init; }
    public string CompanyName { get; init; } = string.Empty;
}
