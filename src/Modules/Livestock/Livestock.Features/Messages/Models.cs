namespace Livestock.Features.Messages;

public record MessageListItem(
    Guid Id,
    Guid ConversationId,
    Guid SenderUserId,
    Guid RecipientUserId,
    string Content,
    bool IsRead,
    DateTime? ReadAt,
    string? AttachmentUrl,
    string? AttachmentType,
    DateTime CreatedAt);

public record MessageDetail(
    Guid Id,
    Guid ConversationId,
    Guid SenderUserId,
    Guid RecipientUserId,
    string Content,
    bool IsRead,
    DateTime? ReadAt,
    string? AttachmentUrl,
    string? AttachmentType,
    DateTime CreatedAt,
    DateTime? UpdatedAt);

public record GetMessageRequest(Guid Id);
public record GetMessagesByConversationRequest(Guid ConversationId, int Page = 1, int PageSize = 50);
public record UpdateMessageRequest(Guid Id, string Content);
public record DeleteMessageRequest(Guid Id);
public record MarkMessageReadRequest(Guid Id);
public record MessagePickItem(Guid Id, string Content, DateTime CreatedAt);
public record MessagePickRequest(Guid? ConversationId, string? Keyword, int Limit = 10);
