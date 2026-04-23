using Shared.Contracts.Integration;

namespace Shared.Contracts.Events.Livestock;

public record MessageReadEvent : IIntegrationEvent
{
    public const string Subject = "livestock.message.read";

    public Guid EventId { get; init; } = Guid.NewGuid();
    public DateTime OccurredAt { get; init; } = DateTime.UtcNow;
    public Guid ConversationId { get; init; }
    public Guid ReadByUserId { get; init; }
    public DateTime ReadAt { get; init; }
}
