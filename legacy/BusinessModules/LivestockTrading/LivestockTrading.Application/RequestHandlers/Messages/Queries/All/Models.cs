namespace LivestockTrading.Application.RequestHandlers.Messages.Queries.All;

public class RequestModel : IRequestModel
{
	public XSorting Sorting { get; set; }
	public List<XFilterItem> Filters { get; set; }
	public XPageRequest PageRequest { get; set; }
}

public class ResponseModel : IResponseModel<Array>
{
	public Guid Id { get; set; }
	public Guid ConversationId { get; set; }
	public Guid SenderUserId { get; set; }
	public Guid RecipientUserId { get; set; }
	public string Content { get; set; }
	public bool IsRead { get; set; }
	public DateTime SentAt { get; set; }
	public DateTime CreatedAt { get; set; }
}
