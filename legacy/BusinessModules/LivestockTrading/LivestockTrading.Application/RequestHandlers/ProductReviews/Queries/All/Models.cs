namespace LivestockTrading.Application.RequestHandlers.ProductReviews.Queries.All;

public class RequestModel : IRequestModel
{
	public XSorting Sorting { get; set; }
	public List<XFilterItem> Filters { get; set; }
	public XPageRequest PageRequest { get; set; }
}

public class ResponseModel : IResponseModel<Array>
{
	public Guid Id { get; set; }
	public Guid ProductId { get; set; }
	public Guid UserId { get; set; }
	public int Rating { get; set; }
	public string Title { get; set; }
	public string Comment { get; set; }
	public bool IsVerifiedPurchase { get; set; }
	public bool IsApproved { get; set; }
	public int HelpfulCount { get; set; }
	public DateTime CreatedAt { get; set; }
}
