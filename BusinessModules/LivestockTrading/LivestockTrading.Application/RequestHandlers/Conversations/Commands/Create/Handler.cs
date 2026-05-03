using Common.Services.Messaging;
using LivestockTrading.Application.Notifications;
using LivestockTrading.Domain.Events;
using Microsoft.Extensions.DependencyInjection;

namespace LivestockTrading.Application.RequestHandlers.Conversations.Commands.Create;

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

		try
		{
			var sp = dependencyProvider.GetInstance<IServiceProvider>();
			_chatNotifier = sp?.GetService<IChatNotifier>();
		}
		catch
		{
			_chatNotifier = null;
		}
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

		// Real-time SignalR broadcast — recipient'in messages listesinde yeni conversation'in
		// aninda gorunmesi icin. Recipient henuz conversation_{id} grubuna join etmemis,
		// bu yuzden user_{recipientUserId} grubuna gondermek gerekiyor.
		if (_chatNotifier != null)
		{
			try
			{
				await _chatNotifier.NotifyConversationCreatedAsync(new ConversationCreatedNotification(
					entity.Id,
					entity.ParticipantUserId1,
					initiatorName,
					entity.ParticipantUserId2,
					entity.ProductId,
					productTitle,
					entity.Subject,
					entity.CreatedAt
				), cancellationToken);
			}
			catch
			{
				// Yutuluyor: SignalR down olsa bile conversation DB'de kayitli, push event'i bagimsiz devam eder
			}
		}

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
