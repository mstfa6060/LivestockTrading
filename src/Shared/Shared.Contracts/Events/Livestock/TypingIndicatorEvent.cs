using Shared.Contracts.Integration;

namespace Shared.Contracts.Events.Livestock;

public record TypingIndicatorEvent : IIntegrationEvent
{
    public const string Subject = "livestock.conversation.typing";

    public Guid EventId { get; init; } = Guid.NewGuid();
    public DateTime OccurredAt { get; init; } = DateTime.UtcNow;
    public Guid ConversationId { get; init; }
    public Guid SenderUserId { get; init; }
    public Guid RecipientUserId { get; init; }
    public bool IsTyping { get; init; }
}
