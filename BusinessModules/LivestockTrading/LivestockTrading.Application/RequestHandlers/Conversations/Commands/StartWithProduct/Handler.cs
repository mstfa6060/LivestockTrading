namespace LivestockTrading.Application.RequestHandlers.Conversations.Commands.StartWithProduct;

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
		var req = (RequestModel)payload;

		// Check if conversation already exists for this product + buyer + seller
		var existingConversation = await _dataAccessLayer.FindExistingConversation(
			req.ProductId, req.BuyerUserId, req.SellerId, cancellationToken);

		Guid conversationId;
		bool isNewConversation;

		if (existingConversation != null)
		{
			conversationId = existingConversation.Id;
			isNewConversation = false;
		}
		else
		{
			// Get seller's UserId
			var seller = await _dataAccessLayer.GetSeller(req.SellerId, cancellationToken);
			if (seller == null)
				throw new ArfBlocksValidationException(
					ErrorCodeGenerator.GetErrorCode(() => LivestockTradingDomainErrors.SellerErrors.SellerNotFound));

			var conversation = new Conversation
			{
				Id = Guid.NewGuid(),
				ParticipantUserId1 = req.BuyerUserId,
				ParticipantUserId2 = seller.UserId,
				ProductId = req.ProductId,
				Subject = await _dataAccessLayer.GetProductTitle(req.ProductId, cancellationToken),
				Status = ConversationStatus.Active,
				LastMessageAt = DateTime.UtcNow,
				CreatedAt = DateTime.UtcNow
			};

			await _dataAccessLayer.AddConversation(conversation, cancellationToken);
			conversationId = conversation.Id;
			isNewConversation = true;

			// Publish notification event
			var initiatorName = _currentUserService.GetCurrentUserDisplayName();
			var productTitle = conversation.Subject;

			await _publisher.PublishFanout("livestocktrading.notification.push", new ConversationCreatedEvent
			{
				ConversationId = conversation.Id,
				InitiatorUserId = req.BuyerUserId,
				InitiatorName = initiatorName,
				RecipientUserId = seller.UserId,
				ProductId = req.ProductId,
				ProductTitle = productTitle,
				Subject = conversation.Subject,
				CreatedAt = conversation.CreatedAt
			});
		}

		// Get seller's UserId for the message recipient
		var sellerForMessage = await _dataAccessLayer.GetSeller(req.SellerId, cancellationToken);

		// Add initial message
		var message = new Message
		{
			Id = Guid.NewGuid(),
			ConversationId = conversationId,
			SenderUserId = req.BuyerUserId,
			RecipientUserId = sellerForMessage.UserId,
			Content = req.InitialMessage,
			SentAt = DateTime.UtcNow,
			CreatedAt = DateTime.UtcNow
		};

		await _dataAccessLayer.AddMessage(message, cancellationToken);

		// Publish message event
		await _publisher.PublishFanout("livestocktrading.notification.push", new MessageCreatedEvent
		{
			MessageId = message.Id,
			ConversationId = message.ConversationId,
			SenderUserId = message.SenderUserId,
			RecipientUserId = message.RecipientUserId,
			SenderName = _currentUserService.GetCurrentUserDisplayName(),
			Content = message.Content,
			CreatedAt = message.SentAt
		});

		return ArfBlocksResults.Success(new ResponseModel
		{
			ConversationId = conversationId,
			IsNewConversation = isNewConversation,
			MessageId = message.Id,
			CreatedAt = message.CreatedAt
		});
	}
}
