namespace LivestockTrading.Application.RequestHandlers.Offers.Queries.All;

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
	public Guid BuyerUserId { get; set; }
	public Guid SellerUserId { get; set; }
	public double OfferedPrice { get; set; }
	public string Currency { get; set; }
	public int Quantity { get; set; }
	public int Status { get; set; }
	public DateTime OfferDate { get; set; }
	public DateTime? ExpiryDate { get; set; }
	public DateTime CreatedAt { get; set; }
}
