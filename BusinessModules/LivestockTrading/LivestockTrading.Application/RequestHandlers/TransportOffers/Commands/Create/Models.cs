namespace LivestockTrading.Application.RequestHandlers.TransportOffers.Commands.Create;

public class RequestModel : IRequestModel
{
	public Guid TransportRequestId { get; set; }
	public Guid TransporterId { get; set; }
	public decimal OfferedPrice { get; set; }
	public string Currency { get; set; }
	public DateTime? EstimatedPickupDate { get; set; }
	public DateTime? EstimatedDeliveryDate { get; set; }
	public int? EstimatedDurationDays { get; set; }
	public string VehicleType { get; set; }
	public bool InsuranceIncluded { get; set; }
	public decimal? InsuranceAmount { get; set; }
	public string AdditionalServices { get; set; }
	public string Message { get; set; }
	public int Status { get; set; }
	public DateTime? ExpiryDate { get; set; }
}

public class ResponseModel : IResponseModel
{
	public Guid Id { get; set; }
	public Guid TransportRequestId { get; set; }
	public Guid TransporterId { get; set; }
	public decimal OfferedPrice { get; set; }
	public string Currency { get; set; }
	public DateTime? EstimatedPickupDate { get; set; }
	public DateTime? EstimatedDeliveryDate { get; set; }
	public int? EstimatedDurationDays { get; set; }
	public string VehicleType { get; set; }
	public bool InsuranceIncluded { get; set; }
	public decimal? InsuranceAmount { get; set; }
	public string AdditionalServices { get; set; }
	public string Message { get; set; }
	public int Status { get; set; }
	public DateTime OfferDate { get; set; }
	public DateTime? ExpiryDate { get; set; }
	public DateTime CreatedAt { get; set; }
}
