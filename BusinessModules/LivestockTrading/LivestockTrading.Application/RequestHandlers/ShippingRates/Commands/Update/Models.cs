namespace LivestockTrading.Application.RequestHandlers.ShippingRates.Commands.Update;

public class RequestModel : IRequestModel
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
}

public class ResponseModel : IResponseModel
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
