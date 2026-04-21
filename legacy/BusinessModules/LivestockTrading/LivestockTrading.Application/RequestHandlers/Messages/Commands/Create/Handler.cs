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

		var entity = mapper.MapToEntity(request);

		await _dataAccessLayer.AddMessage(entity);

		// Get sender name for notification
		var senderName = _currentUserService.GetCurrentUserDisplayName();

		// Publish event for push notification and real-time delivery
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
