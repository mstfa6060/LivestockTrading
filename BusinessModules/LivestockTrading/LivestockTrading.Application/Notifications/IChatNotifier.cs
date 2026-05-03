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

	/// <summary>Mesaj okundu isaretlendiginde conversation'daki diger client'lara broadcast eder.</summary>
	Task NotifyMessageReadAsync(MessageReadNotification notification, CancellationToken cancellationToken = default);

	/// <summary>Yeni conversation acildiginda recipient'in user-bazli grubuna broadcast eder (henuz conversation'a join etmemis olabilir).</summary>
	Task NotifyConversationCreatedAsync(ConversationCreatedNotification notification, CancellationToken cancellationToken = default);
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

public record MessageReadNotification(
	Guid MessageId,
	Guid ConversationId,
	Guid ReadByUserId,
	DateTime ReadAt);

public record ConversationCreatedNotification(
	Guid ConversationId,
	Guid InitiatorUserId,
	string InitiatorName,
	Guid RecipientUserId,
	Guid? ProductId,
	string ProductTitle,
	string Subject,
	DateTime CreatedAt);
