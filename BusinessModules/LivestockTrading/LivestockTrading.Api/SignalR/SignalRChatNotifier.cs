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
}
