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
	public decimal? PowerHp { get; set; }
	public decimal? PowerKw { get; set; }
	public string EngineCapacity { get; set; }
	public decimal? LengthCm { get; set; }
	public decimal? WidthCm { get; set; }
	public decimal? HeightCm { get; set; }
	public decimal? WeightKg { get; set; }
	public decimal? WorkingWidthCm { get; set; }
	public decimal? CapacityLiters { get; set; }
	public decimal? LoadCapacityKg { get; set; }
	public decimal? SpeedKmh { get; set; }
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
