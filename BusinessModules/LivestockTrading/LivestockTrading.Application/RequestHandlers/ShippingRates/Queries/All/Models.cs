namespace LivestockTrading.Application.RequestHandlers.ShippingRates.Queries.All;

public class RequestModel : IRequestModel
{
	public XSorting Sorting { get; set; }
	public List<XFilterItem> Filters { get; set; }
	public XPageRequest PageRequest { get; set; }
}

public class ResponseModel : IResponseModel<Array>
{
	public Guid Id { get; set; }
	public Guid ShippingZoneId { get; set; }
	public Guid? ShippingCarrierId { get; set; }
	public decimal? MinWeight { get; set; }
	public decimal? MaxWeight { get; set; }
	public decimal? MinOrderAmount { get; set; }
	public decimal ShippingCost { get; set; }
	public string Currency { get; set; }
	public int? EstimatedDeliveryDays { get; set; }
	public bool IsFreeShipping { get; set; }
	public bool IsActive { get; set; }
	public DateTime CreatedAt { get; set; }
	public DateTime? UpdatedAt { get; set; }
}
