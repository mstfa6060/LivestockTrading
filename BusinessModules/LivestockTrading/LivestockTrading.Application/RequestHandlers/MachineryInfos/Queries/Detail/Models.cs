namespace LivestockTrading.Application.RequestHandlers.MachineryInfos.Queries.Detail;

public class RequestModel : IRequestModel
{
	public Guid Id { get; set; }
}

public class ResponseModel : IResponseModel
{
	public Guid Id { get; set; }
	public Guid ProductId { get; set; }
	public int Type { get; set; }
	public string Model { get; set; }
	public int? YearOfManufacture { get; set; }
	public string SerialNumber { get; set; }
	public string PowerSource { get; set; }
	public double? PowerHp { get; set; }
	public double? PowerKw { get; set; }
	public string EngineCapacity { get; set; }
	public double? LengthCm { get; set; }
	public double? WidthCm { get; set; }
	public double? HeightCm { get; set; }
	public double? WeightKg { get; set; }
	public double? WorkingWidthCm { get; set; }
	public double? CapacityLiters { get; set; }
	public double? LoadCapacityKg { get; set; }
	public double? SpeedKmh { get; set; }
	public int? HoursUsed { get; set; }
	public DateTime? LastServiceDate { get; set; }
	public string ServiceHistory { get; set; }
	public string IncludedAttachments { get; set; }
	public string CompatibleAttachments { get; set; }
	public bool HasWarranty { get; set; }
	public DateTime? WarrantyExpiryDate { get; set; }
	public string WarrantyDetails { get; set; }
	public string Certifications { get; set; }
	public string SafetyFeatures { get; set; }
	public DateTime CreatedAt { get; set; }
	public DateTime? UpdatedAt { get; set; }
}
