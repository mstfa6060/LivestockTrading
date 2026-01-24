namespace LivestockTrading.Application.RequestHandlers.FeedInfos.Queries.All;

public class RequestModel : IRequestModel
{
	public XSorting Sorting { get; set; }
	public List<XFilterItem> Filters { get; set; }
	public XPageRequest PageRequest { get; set; }
}

public class ResponseModel : IResponseModel<Array>
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
	public DateTime CreatedAt { get; set; }
}
