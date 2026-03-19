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
	public double? MinWeight { get; set; }
	public double? MaxWeight { get; set; }
	public double? MinOrderAmount { get; set; }
	public double ShippingCost { get; set; }
	public string Currency { get; set; }
	public int? EstimatedDeliveryDays { get; set; }
	public bool IsFreeShipping { get; set; }
	public bool IsActive { get; set; }
	public DateTime CreatedAt { get; set; }
	public DateTime? UpdatedAt { get; set; }
}
