using Shared.Contracts.Integration;

namespace Shared.Contracts.Events.Livestock;

public record MessageSentEvent : IIntegrationEvent
{
    public const string Subject = "livestocktrading.notification.push";

    public Guid EventId { get; init; } = Guid.NewGuid();
    public DateTime OccurredAt { get; init; } = DateTime.UtcNow;
    public Guid MessageId { get; init; }
    public Guid ConversationId { get; init; }
    public Guid SenderUserId { get; init; }
    public Guid RecipientUserId { get; init; }
    public string Content { get; init; } = string.Empty;
    public string? AttachmentUrl { get; init; }
}
