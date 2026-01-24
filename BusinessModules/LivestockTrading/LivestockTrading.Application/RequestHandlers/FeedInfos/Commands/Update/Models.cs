namespace LivestockTrading.Application.RequestHandlers.FeedInfos.Commands.Update;

public class RequestModel : IRequestModel
{
	public Guid Id { get; set; }
	public Guid ProductId { get; set; }
	public int Type { get; set; }
	public string TargetAnimal { get; set; }
	public string TargetAge { get; set; }
	public decimal? ProteinPercentage { get; set; }
	public decimal? FatPercentage { get; set; }
	public decimal? FiberPercentage { get; set; }
	public decimal? MoisturePercentage { get; set; }
	public int Form { get; set; }
	public bool IsOrganic { get; set; }
	public bool IsGMOFree { get; set; }
	public bool IsMedicatedFeed { get; set; }
	public string FeedingInstructions { get; set; }
	public string StorageInstructions { get; set; }
	public int? ShelfLifeMonths { get; set; }
	public DateTime? ExpiryDate { get; set; }
	public string BatchNumber { get; set; }
	public string Certifications { get; set; }
}

public class ResponseModel : IResponseModel
{
	public Guid Id { get; set; }
	public Guid ProductId { get; set; }
	public int Type { get; set; }
	public string TargetAnimal { get; set; }
	public string TargetAge { get; set; }
	public decimal? ProteinPercentage { get; set; }
	public decimal? FatPercentage { get; set; }
	public decimal? FiberPercentage { get; set; }
	public decimal? MoisturePercentage { get; set; }
	public int Form { get; set; }
	public bool IsOrganic { get; set; }
	public bool IsGMOFree { get; set; }
	public bool IsMedicatedFeed { get; set; }
	public string FeedingInstructions { get; set; }
	public string StorageInstructions { get; set; }
	public int? ShelfLifeMonths { get; set; }
	public DateTime? ExpiryDate { get; set; }
	public string BatchNumber { get; set; }
	public string Certifications { get; set; }
	public DateTime? UpdatedAt { get; set; }
}
