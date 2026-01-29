namespace LivestockTrading.Application.RequestHandlers.Messages.Commands.SendTypingIndicator;

public class RequestModel : IRequestModel
{
	public Guid ConversationId { get; set; }
	public bool IsTyping { get; set; }
}

public class ResponseModel : IResponseModel
{
	public bool Success { get; set; }
}
