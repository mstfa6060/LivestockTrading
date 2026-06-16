using Livestock.Domain.Enums;

namespace Livestock.Features.Notifications;

public record NotificationItem(Guid Id, NotificationType Type, string Title, string Body, string? ImageUrl, string? DeepLink, bool IsRead, DateTime? ReadAt, Guid? RelatedEntityId, string? RelatedEntityType, DateTime CreatedAt);
public record MarkNotificationReadRequest(Guid Id);
