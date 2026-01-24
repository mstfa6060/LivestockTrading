namespace LivestockTrading.Application.RequestHandlers.ProductViewHistories.Queries.All;

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
	public Guid ProductId { get; set; }
	public DateTime ViewedAt { get; set; }
	public string ViewSource { get; set; }
	public DateTime CreatedAt { get; set; }
}
