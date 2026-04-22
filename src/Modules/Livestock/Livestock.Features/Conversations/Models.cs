using Livestock.Domain.Enums;

namespace Livestock.Features.Conversations;

public record ConversationListItem(Guid Id, Guid InitiatorUserId, Guid RecipientUserId, Guid? ProductId, ConversationStatus Status, DateTime? LastMessageAt, int UnreadCount, DateTime CreatedAt);
public record ConversationDetail(Guid Id, Guid InitiatorUserId, Guid RecipientUserId, Guid? ProductId, ConversationStatus Status, DateTime? LastMessageAt, DateTime CreatedAt);
public record MessageItem(Guid Id, Guid ConversationId, Guid SenderUserId, Guid RecipientUserId, string Content, bool IsRead, DateTime? ReadAt, string? AttachmentUrl, DateTime CreatedAt);

public record CreateConversationRequest(Guid RecipientUserId, Guid? ProductId, string FirstMessage);
public record SendMessageRequest(Guid ConversationId, string Content, string? AttachmentUrl);
public record GetConversationRequest(Guid Id);
public record GetConversationMessagesRequest(Guid ConversationId, int Page = 1, int PageSize = 50);
public record MarkMessagesReadRequest(Guid ConversationId);
public record UnreadCountItem(Guid ConversationId, int UnreadCount);
public record UnreadCountResponse(int Total, List<UnreadCountItem> ByConversation);
