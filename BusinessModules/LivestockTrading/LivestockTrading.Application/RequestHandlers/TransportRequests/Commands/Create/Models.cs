namespace LivestockTrading.Application.RequestHandlers.TransportRequests.Commands.Create;

public class RequestModel : IRequestModel
{
	public Guid ProductId { get; set; }
	public Guid SellerId { get; set; }
	public Guid BuyerId { get; set; }
	public double? AgreedPrice { get; set; }
	public string Currency { get; set; }
	public Guid PickupLocationId { get; set; }
	public Guid DeliveryLocationId { get; set; }
	public double? EstimatedDistanceKm { get; set; }
	public double? WeightKg { get; set; }
	public double? VolumeCubicMeters { get; set; }
	public string SpecialInstructions { get; set; }
	public DateTime? PreferredPickupDate { get; set; }
	public DateTime? PreferredDeliveryDate { get; set; }
	public bool IsUrgent { get; set; }
	public int TransportType { get; set; }
	public int Status { get; set; }
	public string Notes { get; set; }
}

public class ResponseModel : IResponseModel
{
	public Guid Id { get; set; }
	public Guid ProductId { get; set; }
	public Guid SellerId { get; set; }
	public Guid BuyerId { get; set; }
	public double? AgreedPrice { get; set; }
	public string Currency { get; set; }
	public Guid PickupLocationId { get; set; }
	public Guid DeliveryLocationId { get; set; }
	public double? EstimatedDistanceKm { get; set; }
	public double? WeightKg { get; set; }
	public double? VolumeCubicMeters { get; set; }
	public string SpecialInstructions { get; set; }
	public DateTime? PreferredPickupDate { get; set; }
	public DateTime? PreferredDeliveryDate { get; set; }
	public bool IsUrgent { get; set; }
	public int TransportType { get; set; }
	public int Status { get; set; }
	public string Notes { get; set; }
	public DateTime CreatedAt { get; set; }
}
