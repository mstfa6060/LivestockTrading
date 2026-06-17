using Shared.Contracts.Integration;

namespace Shared.Contracts.Events.Livestock;

public record ConversationCreatedEvent : IIntegrationEvent
{
    public const string Subject = "livestock.conversation.created";

    public Guid EventId { get; init; } = Guid.NewGuid();
    public DateTime OccurredAt { get; init; } = DateTime.UtcNow;
    public Guid ConversationId { get; init; }
    public Guid InitiatorUserId { get; init; }
    public Guid RecipientUserId { get; init; }
    public Guid? ProductId { get; init; }
}
