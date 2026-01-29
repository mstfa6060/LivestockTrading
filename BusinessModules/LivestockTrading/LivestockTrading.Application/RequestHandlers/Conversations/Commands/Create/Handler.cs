using Common.Services.Messaging;
using LivestockTrading.Domain.Events;

namespace LivestockTrading.Application.RequestHandlers.Conversations.Commands.Create;

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

		await _dataAccessLayer.AddConversation(entity);

		// Get initiator name and product title for notification
		var initiatorName = _currentUserService.GetCurrentUserDisplayName();
		var productTitle = await _dataAccessLayer.GetProductTitle(entity.ProductId, cancellationToken);

		// Publish event for push notification to recipient
		await _publisher.PublishFanout("livestocktrading.notification.push", new ConversationCreatedEvent
		{
			ConversationId = entity.Id,
			InitiatorUserId = entity.ParticipantUserId1,
			InitiatorName = initiatorName,
			RecipientUserId = entity.ParticipantUserId2,
			ProductId = entity.ProductId,
			ProductTitle = productTitle,
			Subject = entity.Subject,
			CreatedAt = entity.CreatedAt
		});

		var response = mapper.MapToResponse(entity);
		return ArfBlocksResults.Success(response);
	}
}
