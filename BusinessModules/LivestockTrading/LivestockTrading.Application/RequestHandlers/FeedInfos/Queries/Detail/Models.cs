namespace LivestockTrading.Application.RequestHandlers.FeedInfos.Queries.Detail;

public class RequestModel : IRequestModel
{
	public Guid Id { get; set; }
}

public class ResponseModel : IResponseModel
{
	public Guid Id { get; set; }
	public Guid ProductId { get; set; }
	public int Type { get; set; }
	public string TargetAnimal { get; set; }
	public string TargetAge { get; set; }
	public double? ProteinPercentage { get; set; }
	public double? FatPercentage { get; set; }
	public double? FiberPercentage { get; set; }
	public double? MoisturePercentage { get; set; }
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
	public DateTime CreatedAt { get; set; }
	public DateTime? UpdatedAt { get; set; }
}
