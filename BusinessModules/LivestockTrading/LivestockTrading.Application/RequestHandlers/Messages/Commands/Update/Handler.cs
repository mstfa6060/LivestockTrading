using Common.Services.Messaging;
using LivestockTrading.Domain.Events;

namespace LivestockTrading.Application.RequestHandlers.Messages.Commands.Update;

public class Handler : IRequestHandler
{
	private readonly DataAccess _dataAccessLayer;
	private readonly IRabbitMqPublisher _publisher;

	public Handler(ArfBlocksDependencyProvider dependencyProvider, object dataAccess)
	{
		_dataAccessLayer = (DataAccess)dataAccess;
		_publisher = dependencyProvider.GetInstance<IRabbitMqPublisher>();
	}

	public async Task<ArfBlocksRequestResult> Handle(IRequestModel payload, EndpointContext context, CancellationToken cancellationToken)
	{
		var request = (RequestModel)payload;
		var mapper = new Mapper();

		var message = await _dataAccessLayer.GetMessageById(request.Id);

		if (message == null)
			throw new ArfBlocksValidationException(
				ErrorCodeGenerator.GetErrorCode(() => LivestockTradingDomainErrors.CommonErrors.IdNotValid));

		// Check if message is being marked as read
		var wasUnread = !message.IsRead;
		var isBeingMarkedAsRead = request.IsRead && wasUnread;

		mapper.MapToEntity(request, message);

		await _dataAccessLayer.SaveChanges();

		// Publish read event if message was marked as read
		if (isBeingMarkedAsRead)
		{
			await _publisher.PublishFanout("livestocktrading.notification.push", new MessageReadEvent
			{
				MessageId = message.Id,
				ConversationId = message.ConversationId,
				ReadByUserId = message.RecipientUserId,
				ReadAt = message.ReadAt ?? DateTime.UtcNow
			});
		}

		var response = mapper.MapToResponse(message);
		return ArfBlocksResults.Success(response);
	}
}
