using LivestockTrading.Api.Hubs;
using LivestockTrading.Application.Notifications;
using Microsoft.AspNetCore.SignalR;
using Serilog;

namespace LivestockTrading.Api.SignalR;

public class SignalRChatNotifier : IChatNotifier
{
	private readonly IHubContext<ChatHub> _hubContext;

	public SignalRChatNotifier(IHubContext<ChatHub> hubContext)
	{
		_hubContext = hubContext;
	}

	public async Task NotifyMessageCreatedAsync(MessageCreatedNotification n, CancellationToken cancellationToken = default)
	{
		// Hem conversation_{id} grubuna (chat ekrani acik olanlar) hem de katilimcilarin user_{userId}
		// gruplarina gonderilir. Sebep: conversation listesinde duran ama o conversation'a join etmemis
		// kullanicilar mesaj listesi grubunda olmaz, sadece kendi user grubunda olur. user_{userId}'e
		// yansitilmazsa Messages list ekraninda lastMessage/unreadCount real-time guncellenmez (E2E ile
		// kanitlandi). Cift teslim icin frontend useChat handleNewMessage duplicate guard tutar.
		var payload = new
		{
			id = n.Id,
			conversationId = n.ConversationId,
			senderUserId = n.SenderUserId,
			recipientUserId = n.RecipientUserId,
			content = n.Content,
			attachmentUrls = n.AttachmentUrls,
			sentAt = n.SentAt,
			createdAt = n.CreatedAt,
			isRead = false
		};

		try
		{
			await _hubContext.Clients.Groups(
				$"conversation_{n.ConversationId}",
				$"user_{n.RecipientUserId}",
				$"user_{n.SenderUserId}")
				.SendAsync("ReceiveMessage", payload, cancellationToken);
		}
		catch (Exception ex)
		{
			Log.Warning(ex, "SignalR ReceiveMessage broadcast failed for conversation {ConversationId}", n.ConversationId);
		}
	}

	public async Task NotifyMessageReadAsync(MessageReadNotification n, CancellationToken cancellationToken = default)
	{
		// Sender'in (artik recipient'i degil) listesinde de okundu tikinin tazelenmesi icin
		// her iki katilimcinin user grubuna da gonderiyoruz. ReadByUserId burada okuyan kisi —
		// karsi taraf icin: conversation katilimcilarini hub'da bilemiyoruz, ama ConversationId'den
		// turetmek icin DB'ye git lazim. Pratik cozum: tum dinleyiciler (sender ile recipient) ayni
		// conversation_{id} grubuna ek olarak okuyanin user grubuna; sender her iki kanal da olabilir.
		try
		{
			await _hubContext.Clients.Groups(
				$"conversation_{n.ConversationId}",
				$"user_{n.ReadByUserId}")
				.SendAsync("MessageRead", new
				{
					messageId = n.MessageId,
					conversationId = n.ConversationId,
					readByUserId = n.ReadByUserId,
					readAt = n.ReadAt
				}, cancellationToken);
		}
		catch (Exception ex)
		{
			Log.Warning(ex, "SignalR MessageRead broadcast failed for conversation {ConversationId}", n.ConversationId);
		}
	}

	public async Task NotifyConversationCreatedAsync(ConversationCreatedNotification n, CancellationToken cancellationToken = default)
	{
		// Recipient henuz conversation_{id} grubuna join etmemis olabilir (chat sayfasi acik degil),
		// bu yuzden user-specific gruba (`user_{recipientUserId}`) gondermek gerekiyor.
		// ChatHub.OnConnectedAsync her authenticated connection'i bu gruba otomatik ekler.
		try
		{
			await _hubContext.Clients.Group($"user_{n.RecipientUserId}")
				.SendAsync("ConversationCreated", new
				{
					conversationId = n.ConversationId,
					initiatorUserId = n.InitiatorUserId,
					initiatorName = n.InitiatorName,
					recipientUserId = n.RecipientUserId,
					productId = n.ProductId,
					productTitle = n.ProductTitle,
					subject = n.Subject,
					createdAt = n.CreatedAt
				}, cancellationToken);
		}
		catch (Exception ex)
		{
			Log.Warning(ex, "SignalR ConversationCreated broadcast failed for recipient {RecipientUserId}", n.RecipientUserId);
		}
	}
}
