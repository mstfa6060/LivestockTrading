namespace LivestockTrading.Application.RequestHandlers.VeterinaryInfos.Queries.All;

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
	public string TherapeuticCategory { get; set; }
	public string TargetSpecies { get; set; }
	public string Indications { get; set; }
	public string ActiveIngredients { get; set; }
	public string Strength { get; set; }
	public int Route { get; set; }
	public string DosageInstructions { get; set; }
	public bool RequiresPrescription { get; set; }
	public string RegistrationNumber { get; set; }
	public string Contraindications { get; set; }
	public int? MeatWithdrawalDays { get; set; }
	public int? MilkWithdrawalDays { get; set; }
	public int? EggWithdrawalDays { get; set; }
	public string StorageInstructions { get; set; }
	public int? ShelfLifeMonths { get; set; }
	public DateTime? ExpiryDate { get; set; }
	public string BatchNumber { get; set; }
	public bool RequiresColdChain { get; set; }
	public string Certifications { get; set; }
	public DateTime CreatedAt { get; set; }
}
