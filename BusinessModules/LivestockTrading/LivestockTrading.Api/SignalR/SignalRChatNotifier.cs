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
		// Broadcast hatasi mesaj gonderme akisini bozmasin (Redis down vs.) — log'la, swallow et.
		try
		{
			await _hubContext.Clients.Group($"conversation_{n.ConversationId}")
				.SendAsync("ReceiveMessage", new
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
				}, cancellationToken);
		}
		catch (Exception ex)
		{
			Log.Warning(ex, "SignalR ReceiveMessage broadcast failed for conversation {ConversationId}", n.ConversationId);
		}
	}

	public async Task NotifyMessageReadAsync(MessageReadNotification n, CancellationToken cancellationToken = default)
	{
		try
		{
			await _hubContext.Clients.Group($"conversation_{n.ConversationId}")
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
