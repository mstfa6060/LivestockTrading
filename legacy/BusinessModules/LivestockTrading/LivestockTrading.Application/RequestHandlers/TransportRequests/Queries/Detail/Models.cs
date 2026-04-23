namespace LivestockTrading.Application.RequestHandlers.TransportRequests.Queries.Detail;

public class RequestModel : IRequestModel
{
	public Guid Id { get; set; }
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
	public bool IsInPool { get; set; }
	public DateTime? AddedToPoolAt { get; set; }
	public Guid? AssignedTransporterId { get; set; }
	public DateTime? AssignedAt { get; set; }
	public DateTime? PickedUpAt { get; set; }
	public DateTime? DeliveredAt { get; set; }
	public DateTime? CancelledAt { get; set; }
	public string CancellationReason { get; set; }
	public string Notes { get; set; }
	public DateTime CreatedAt { get; set; }
	public DateTime? UpdatedAt { get; set; }
}
