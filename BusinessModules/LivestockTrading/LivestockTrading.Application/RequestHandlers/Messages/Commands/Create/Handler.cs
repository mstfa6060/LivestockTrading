using Common.Services.Messaging;
using LivestockTrading.Application.Notifications;
using LivestockTrading.Domain.Events;

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
		_chatNotifier = dependencyProvider.GetInstance<IChatNotifier>();
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
