namespace LivestockTrading.Application.RequestHandlers.SearchHistories.Queries.All;

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
	public string SearchQuery { get; set; }
	public string Filters { get; set; }
	public int ResultsCount { get; set; }
	public DateTime SearchedAt { get; set; }
	public DateTime CreatedAt { get; set; }
}
