namespace LivestockTrading.Domain.Events;

/// <summary>
/// Event fired when a new message is created
/// Used to trigger push notifications and real-time updates
/// </summary>
public class MessageCreatedEvent : IDomainEvent
{
    public Guid MessageId { get; set; }
    public Guid ConversationId { get; set; }
    public Guid SenderUserId { get; set; }
    public Guid RecipientUserId { get; set; }
    public string SenderName { get; set; }
    public string Content { get; set; }
    public string AttachmentUrls { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime OccurredAt => CreatedAt;
}

/// <summary>
/// Event fired when a message is marked as read
/// Used to update read receipts in real-time
/// </summary>
public class MessageReadEvent : IDomainEvent
{
    public Guid MessageId { get; set; }
    public Guid ConversationId { get; set; }
    public Guid ReadByUserId { get; set; }
    public DateTime ReadAt { get; set; } = DateTime.UtcNow;
    public DateTime OccurredAt => ReadAt;
}

/// <summary>
/// Event fired when user starts/stops typing
/// Used for typing indicator feature
/// </summary>
public class TypingIndicatorEvent : IDomainEvent
{
    public Guid ConversationId { get; set; }
    public Guid UserId { get; set; }
    public string UserName { get; set; }
    public bool IsTyping { get; set; }
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    public DateTime OccurredAt => Timestamp;
}

/// <summary>
/// Event fired when a new conversation is created
/// Used to notify the recipient about new conversation
/// </summary>
public class ConversationCreatedEvent : IDomainEvent
{
    public Guid ConversationId { get; set; }
    public Guid InitiatorUserId { get; set; }
    public string InitiatorName { get; set; }
    public Guid RecipientUserId { get; set; }
    public Guid? ProductId { get; set; }
    public string ProductTitle { get; set; }
    public string Subject { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime OccurredAt => CreatedAt;
}

/// <summary>
/// Event fired when user comes online
/// </summary>
public class UserOnlineEvent : IDomainEvent
{
    public Guid UserId { get; set; }
    public string ConnectionId { get; set; }
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    public DateTime OccurredAt => Timestamp;
}

/// <summary>
/// Event fired when user goes offline
/// </summary>
public class UserOfflineEvent : IDomainEvent
{
    public Guid UserId { get; set; }
    public string ConnectionId { get; set; }
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    public DateTime OccurredAt => Timestamp;
}
