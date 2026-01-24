namespace LivestockTrading.Application.RequestHandlers.SellerReviews.Queries.All;

public class RequestModel : IRequestModel
{
	public XSorting Sorting { get; set; }
	public List<XFilterItem> Filters { get; set; }
	public XPageRequest PageRequest { get; set; }
}

public class ResponseModel : IResponseModel<Array>
{
	public Guid Id { get; set; }
	public Guid SellerId { get; set; }
	public Guid UserId { get; set; }
	public int OverallRating { get; set; }
	public int CommunicationRating { get; set; }
	public int ShippingSpeedRating { get; set; }
	public int ProductQualityRating { get; set; }
	public string Title { get; set; }
	public bool IsVerifiedPurchase { get; set; }
	public bool IsApproved { get; set; }
	public DateTime CreatedAt { get; set; }
}
