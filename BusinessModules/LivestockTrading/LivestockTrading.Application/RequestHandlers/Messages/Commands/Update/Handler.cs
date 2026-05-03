using Common.Services.Messaging;
using LivestockTrading.Application.Notifications;
using LivestockTrading.Domain.Events;
using Microsoft.Extensions.DependencyInjection;

namespace LivestockTrading.Application.RequestHandlers.Messages.Commands.Update;

public class Handler : IRequestHandler
{
	private readonly DataAccess _dataAccessLayer;
	private readonly IRabbitMqPublisher _publisher;
	private readonly IChatNotifier _chatNotifier;

	public Handler(ArfBlocksDependencyProvider dependencyProvider, object dataAccess)
	{
		_dataAccessLayer = (DataAccess)dataAccess;
		_publisher = dependencyProvider.GetInstance<IRabbitMqPublisher>();

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
			var readAt = message.ReadAt ?? DateTime.UtcNow;

			// Real-time SignalR broadcast — sender'in chat ekraninda "okundu" tikini gostermek icin
			if (_chatNotifier != null)
			{
				try
				{
					await _chatNotifier.NotifyMessageReadAsync(new MessageReadNotification(
						message.Id,
						message.ConversationId,
						message.RecipientUserId,
						readAt
					), cancellationToken);
				}
				catch
				{
					// Yutuluyor: SignalR down olsa bile DB'de kayitli, push event'i bagimsiz devam eder
				}
			}

			await _publisher.PublishFanout("livestocktrading.notification.push", new MessageReadEvent
			{
				MessageId = message.Id,
				ConversationId = message.ConversationId,
				ReadByUserId = message.RecipientUserId,
				ReadAt = readAt
			});
		}

		var response = mapper.MapToResponse(message);
		return ArfBlocksResults.Success(response);
	}
}
