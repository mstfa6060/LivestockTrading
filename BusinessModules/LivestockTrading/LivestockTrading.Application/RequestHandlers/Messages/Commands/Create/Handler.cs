using Common.Services.Messaging;
using LivestockTrading.Domain.Events;

namespace LivestockTrading.Application.RequestHandlers.Messages.Commands.Create;

public class Handler : IRequestHandler
{
	private readonly DataAccess _dataAccessLayer;
	private readonly IRabbitMqPublisher _publisher;
	private readonly CurrentUserService _currentUserService;

	public Handler(ArfBlocksDependencyProvider dependencyProvider, object dataAccess)
	{
		_dataAccessLayer = (DataAccess)dataAccess;
		_publisher = dependencyProvider.GetInstance<IRabbitMqPublisher>();
		_currentUserService = dependencyProvider.GetInstance<CurrentUserService>();
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

		var senderName = _currentUserService.GetCurrentUserDisplayName();

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
