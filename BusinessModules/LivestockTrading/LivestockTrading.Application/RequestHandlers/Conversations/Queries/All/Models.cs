namespace LivestockTrading.Application.RequestHandlers.Conversations.Queries.All;

public class RequestModel : IRequestModel
{
	public XSorting Sorting { get; set; }
	public List<XFilterItem> Filters { get; set; }
	public XPageRequest PageRequest { get; set; }
}

public class ResponseModel : IResponseModel<Array>
{
	public Guid Id { get; set; }
	public Guid ParticipantUserId1 { get; set; }
	public Guid ParticipantUserId2 { get; set; }
	public Guid? ProductId { get; set; }
	public string Subject { get; set; }
	public int Status { get; set; }
	public DateTime? LastMessageAt { get; set; }
	public DateTime CreatedAt { get; set; }
}
