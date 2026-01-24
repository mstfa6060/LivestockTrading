namespace LivestockTrading.Application.RequestHandlers.Deals.Queries.All;

public class RequestModel : IRequestModel
{
	public XSorting Sorting { get; set; }
	public List<XFilterItem> Filters { get; set; }
	public XPageRequest PageRequest { get; set; }
}

public class ResponseModel : IResponseModel<Array>
{
	public Guid Id { get; set; }
	public string DealNumber { get; set; }
	public Guid ProductId { get; set; }
	public Guid SellerId { get; set; }
	public Guid BuyerId { get; set; }
	public decimal AgreedPrice { get; set; }
	public string Currency { get; set; }
	public int Quantity { get; set; }
	public int Status { get; set; }
	public DateTime DealDate { get; set; }
	public int DeliveryMethod { get; set; }
	public bool IsCompleted { get; set; }
	public bool IsCancelled { get; set; }
	public DateTime CreatedAt { get; set; }
}
