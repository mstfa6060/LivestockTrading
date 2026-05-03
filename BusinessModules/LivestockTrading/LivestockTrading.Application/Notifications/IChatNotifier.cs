namespace LivestockTrading.Application.Notifications;

/// <summary>
/// Real-time chat broadcast'leri (SignalR ChatHub) icin Application-level abstraction.
/// Implementation Api projesinde (SignalRChatNotifier); ChatHub.IHubContext sadece API process'inde
/// erisilebildigi icin Application'da interface, Api'de concrete sinif pattern'i kullaniliyor.
/// </summary>
public interface IChatNotifier
{
	/// <summary>Yeni mesaj olusturuldugunda conversation'a abone olan tum client'lara broadcast eder.</summary>
	Task NotifyMessageCreatedAsync(MessageCreatedNotification notification, CancellationToken cancellationToken = default);
}

public record MessageCreatedNotification(
	Guid Id,
	Guid ConversationId,
	Guid SenderUserId,
	Guid RecipientUserId,
	string Content,
	string AttachmentUrls,
	DateTime SentAt,
	DateTime CreatedAt);
