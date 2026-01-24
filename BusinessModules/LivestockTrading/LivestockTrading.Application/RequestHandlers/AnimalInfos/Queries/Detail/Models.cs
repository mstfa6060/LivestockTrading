namespace LivestockTrading.Application.RequestHandlers.AnimalInfos.Queries.Detail;

public class RequestModel : IRequestModel
{
	public Guid Id { get; set; }
}

public class ResponseModel : IResponseModel
{
	public Guid Id { get; set; }
	public Guid ProductId { get; set; }
	public string BreedName { get; set; }
	public int Gender { get; set; }
	public DateTime? DateOfBirth { get; set; }
	public int? AgeMonths { get; set; }
	public decimal? WeightKg { get; set; }
	public decimal? HeightCm { get; set; }
	public string Color { get; set; }
	public string Markings { get; set; }
	public string TagNumber { get; set; }
	public string MicrochipNumber { get; set; }
	public string PassportNumber { get; set; }
	public string RegistrationNumber { get; set; }
	public int HealthStatus { get; set; }
	public DateTime? LastHealthCheckDate { get; set; }
	public bool IsPregnant { get; set; }
	public DateTime? ExpectedDueDate { get; set; }
	public int? NumberOfBirths { get; set; }
	public decimal? DailyMilkProductionLiters { get; set; }
	public decimal? AverageDailyEggProduction { get; set; }
	public string SireDetails { get; set; }
	public string DamDetails { get; set; }
	public int Purpose { get; set; }
	public DateTime CreatedAt { get; set; }
	public DateTime? UpdatedAt { get; set; }
}
