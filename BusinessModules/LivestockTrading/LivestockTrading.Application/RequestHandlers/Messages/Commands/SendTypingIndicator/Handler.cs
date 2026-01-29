using Common.Services.Messaging;
using LivestockTrading.Domain.Events;

namespace LivestockTrading.Application.RequestHandlers.Messages.Commands.SendTypingIndicator;

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
		var currentUser = _currentUserService.GetCurrentUser();

		// Publish typing indicator event for real-time delivery via SignalR
		await _publisher.PublishFanout("livestocktrading.notification.push", new TypingIndicatorEvent
		{
			ConversationId = request.ConversationId,
			UserId = currentUser.Id,
			UserName = currentUser.DisplayName,
			IsTyping = request.IsTyping,
			Timestamp = DateTime.UtcNow
		});

		var response = mapper.MapToResponse(true);
		return ArfBlocksResults.Success(response);
	}
}
