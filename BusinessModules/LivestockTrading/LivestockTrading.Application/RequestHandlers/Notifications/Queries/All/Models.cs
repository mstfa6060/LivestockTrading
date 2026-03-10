namespace LivestockTrading.Application.RequestHandlers.Notifications.Queries.All;

public class RequestModel : IRequestModel
{
	public XSorting Sorting { get; set; }
	public List<XFilterItem> Filters { get; set; }
	public XPageRequest PageRequest { get; set; }
}

public class ResponseModel : IResponseModel<Array>
{
	public Guid Id { get; set; }
	public Guid UserId { get; set; }
	public string Title { get; set; }
	public string Message { get; set; }
	public int Type { get; set; }
	public string ActionUrl { get; set; }
	public string ActionData { get; set; }
	public bool IsRead { get; set; }
	public DateTime SentAt { get; set; }
	public DateTime CreatedAt { get; set; }
}
