using Common.Services.Messaging;
using LivestockTrading.Application.Notifications;
using LivestockTrading.Domain.Events;
using Microsoft.Extensions.DependencyInjection;

namespace LivestockTrading.Application.RequestHandlers.Messages.Commands.Create;

public class Handler : IRequestHandler
{
	private readonly DataAccess _dataAccessLayer;
	private readonly IRabbitMqPublisher _publisher;
	private readonly CurrentUserService _currentUserService;
	private readonly IChatNotifier _chatNotifier;

	public Handler(ArfBlocksDependencyProvider dependencyProvider, object dataAccess)
	{
		_dataAccessLayer = (DataAccess)dataAccess;
		_publisher = dependencyProvider.GetInstance<IRabbitMqPublisher>();
		_currentUserService = dependencyProvider.GetInstance<CurrentUserService>();

		// IChatNotifier Application interface, implementation Api'de.
		// ArfBlocks ApplicationDependencyProvider Api'yi import edemez (dependency direction),
		// bu yüzden runtime'da Microsoft.Extensions.DependencyInjection.IServiceProvider üzerinden resolve.
		try
		{
			var sp = dependencyProvider.GetInstance<IServiceProvider>();
			Console.WriteLine($"[ChatNotifier diag] ServiceProvider null? {sp == null}");
			_chatNotifier = sp?.GetService<IChatNotifier>();
			Console.WriteLine($"[ChatNotifier diag] ChatNotifier null? {_chatNotifier == null} (type: {_chatNotifier?.GetType().FullName})");
		}
		catch (Exception ex)
		{
			Console.WriteLine($"[ChatNotifier diag] Exception during resolve: {ex.GetType().Name}: {ex.Message}");
			_chatNotifier = null;
		}
	}

	public async Task<ArfBlocksRequestResult> Handle(IRequestModel payload, EndpointContext context, CancellationToken cancellationToken)
	{
		var request = (RequestModel)payload;
		var mapper = new Mapper();

		// Sender always comes from JWT — never trust the body
		var senderUserId = _currentUserService.GetCurrentUserId();

		// Recipient is the OTHER participant of the conversation; resolved server-side
		var conversation = await _dataAccessLayer.GetConversationForUpdate(request.ConversationId, cancellationToken);
		var recipientUserId = conversation.ParticipantUserId1 == senderUserId
			? conversation.ParticipantUserId2
			: conversation.ParticipantUserId1;

		var entity = mapper.MapToEntity(request, senderUserId, recipientUserId);

		// Same SaveChanges updates Conversation.LastMessageAt so list view stays sorted correctly
		await _dataAccessLayer.AddMessageAndTouchConversation(entity, conversation, cancellationToken);

		// Real-time SignalR broadcast (chat ekrani acik olan client'lar icin)
		if (_chatNotifier != null)
		{
			try
			{
				Console.WriteLine($"[ChatNotifier diag] Calling NotifyMessageCreatedAsync for message {entity.Id} conversation {entity.ConversationId}");
				await _chatNotifier.NotifyMessageCreatedAsync(new MessageCreatedNotification(
					entity.Id,
					entity.ConversationId,
					entity.SenderUserId,
					entity.RecipientUserId,
					entity.Content,
					entity.AttachmentUrls,
					entity.SentAt,
					entity.CreatedAt
				), cancellationToken);
				Console.WriteLine($"[ChatNotifier diag] Broadcast completed for message {entity.Id}");
			}
			catch (Exception ex)
			{
				// Broadcast hatasi mesaj kaydini bozmasin (Redis/Hub down vs.)
				Console.WriteLine($"[ChatNotifier diag] SignalR broadcast FAILED for message {entity.Id}: {ex.GetType().Name}: {ex.Message}");
			}
		}
		else
		{
			Console.WriteLine($"[ChatNotifier diag] SKIPPING broadcast — _chatNotifier is NULL (message {entity.Id})");
		}

		var senderName = _currentUserService.GetCurrentUserDisplayName();

		// Push notification icin event publish (kapali uygulamalarda Firebase ile bildirim)
		await _publisher.PublishFanout("livestocktrading.notification.push", new MessageCreatedEvent
		{
			MessageId = entity.Id,
			ConversationId = entity.ConversationId,
			SenderUserId = entity.SenderUserId,
			RecipientUserId = entity.RecipientUserId,
			SenderName = senderName,
			Content = entity.Content,
			AttachmentUrls = entity.AttachmentUrls,
			CreatedAt = entity.SentAt
		});

		var response = mapper.MapToResponse(entity);
		return ArfBlocksResults.Success(response);
	}
}
