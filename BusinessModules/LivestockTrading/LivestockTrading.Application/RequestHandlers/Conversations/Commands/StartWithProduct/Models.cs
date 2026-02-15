namespace LivestockTrading.Application.RequestHandlers.Conversations.Commands.StartWithProduct;

public class RequestModel : IRequestModel
{
	public Guid ProductId { get; set; }
	public Guid SellerId { get; set; }
	public Guid BuyerUserId { get; set; }
	public string InitialMessage { get; set; }
}

public class ResponseModel : IResponseModel
{
	public Guid ConversationId { get; set; }
	public bool IsNewConversation { get; set; }
	public Guid MessageId { get; set; }
	public DateTime CreatedAt { get; set; }
}
