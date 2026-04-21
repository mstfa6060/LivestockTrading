namespace LivestockTrading.Application.RequestHandlers.ChemicalInfos.Queries.Detail;

public class RequestModel : IRequestModel
{
	public Guid Id { get; set; }
}

public class ResponseModel : IResponseModel
{
	public Guid Id { get; set; }
	public Guid ProductId { get; set; }
	public int Type { get; set; }
	public string SubType { get; set; }
	public string ActiveIngredients { get; set; }
	public string InertIngredients { get; set; }
	public string ChemicalFormula { get; set; }
	public string RegistrationNumber { get; set; }
	public string ApprovalAgency { get; set; }
	public DateTime? RegistrationDate { get; set; }
	public DateTime? ExpiryDate { get; set; }
	public string ApplicationMethod { get; set; }
	public string TargetPests { get; set; }
	public string TargetCrops { get; set; }
	public string DosageInstructions { get; set; }
	public int ToxicityLevel { get; set; }
	public string SafetyInstructions { get; set; }
	public string FirstAidInstructions { get; set; }
	public int? ReEntryIntervalHours { get; set; }
	public int? PreHarvestIntervalDays { get; set; }
	public bool IsOrganic { get; set; }
	public bool IsBiodegradable { get; set; }
	public string EnvironmentalImpact { get; set; }
	public string StorageInstructions { get; set; }
	public string StorageTemperature { get; set; }
	public int? ShelfLifeMonths { get; set; }
	public string Certifications { get; set; }
	public DateTime CreatedAt { get; set; }
	public DateTime? UpdatedAt { get; set; }
}
