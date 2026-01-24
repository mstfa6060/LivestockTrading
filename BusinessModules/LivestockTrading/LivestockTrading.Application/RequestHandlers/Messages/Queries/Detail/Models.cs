namespace LivestockTrading.Application.RequestHandlers.Messages.Queries.Detail;

public class RequestModel : IRequestModel
{
	public Guid Id { get; set; }
}

public class ResponseModel : IResponseModel
{
	public Guid Id { get; set; }
	public Guid ConversationId { get; set; }
	public Guid SenderUserId { get; set; }
	public Guid RecipientUserId { get; set; }
	public string Content { get; set; }
	public string AttachmentUrls { get; set; }
	public bool IsRead { get; set; }
	public DateTime? ReadAt { get; set; }
	public DateTime SentAt { get; set; }
	public DateTime CreatedAt { get; set; }
}
