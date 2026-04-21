namespace LivestockTrading.Application.RequestHandlers.SeedInfos.Commands.Update;

public class RequestModel : IRequestModel
{
	public Guid Id { get; set; }
	public Guid ProductId { get; set; }
	public int Type { get; set; }
	public string Variety { get; set; }
	public string ScientificName { get; set; }
	public string CommonNames { get; set; }
	public string SeedSize { get; set; }
	public string SeedColor { get; set; }
	public double? GerminationRate { get; set; }
	public int? GerminationDays { get; set; }
	public string ClimateZones { get; set; }
	public string SoilType { get; set; }
	public string SunlightRequirement { get; set; }
	public string WaterRequirement { get; set; }
	public string PlantingDepthCm { get; set; }
	public string SpacingCm { get; set; }
	public int? DaysToMaturity { get; set; }
	public string PlantingSeason { get; set; }
	public string HarvestSeason { get; set; }
	public string ExpectedYield { get; set; }
	public string YieldUnit { get; set; }
	public string PlantHeightCm { get; set; }
	public string PlantSpreadCm { get; set; }
	public string FlowerColor { get; set; }
	public string FruitSize { get; set; }
	public string DiseaseResistance { get; set; }
	public string PestResistance { get; set; }
	public bool IsDroughtTolerant { get; set; }
	public bool IsFrostTolerant { get; set; }
	public bool IsHybrid { get; set; }
	public bool IsHeirloom { get; set; }
	public bool IsGMO { get; set; }
	public bool IsOrganic { get; set; }
	public bool IsTreated { get; set; }
	public string Certifications { get; set; }
	public DateTime? TestDate { get; set; }
	public string LotNumber { get; set; }
	public DateTime? PackageDate { get; set; }
	public DateTime? ExpiryDate { get; set; }
	public string StorageInstructions { get; set; }
}

public class ResponseModel : IResponseModel
{
	public Guid Id { get; set; }
	public Guid ProductId { get; set; }
	public int Type { get; set; }
	public string Variety { get; set; }
	public string ScientificName { get; set; }
	public string CommonNames { get; set; }
	public string SeedSize { get; set; }
	public string SeedColor { get; set; }
	public double? GerminationRate { get; set; }
	public int? GerminationDays { get; set; }
	public string ClimateZones { get; set; }
	public string SoilType { get; set; }
	public string SunlightRequirement { get; set; }
	public string WaterRequirement { get; set; }
	public string PlantingDepthCm { get; set; }
	public string SpacingCm { get; set; }
	public int? DaysToMaturity { get; set; }
	public string PlantingSeason { get; set; }
	public string HarvestSeason { get; set; }
	public string ExpectedYield { get; set; }
	public string YieldUnit { get; set; }
	public string PlantHeightCm { get; set; }
	public string PlantSpreadCm { get; set; }
	public string FlowerColor { get; set; }
	public string FruitSize { get; set; }
	public string DiseaseResistance { get; set; }
	public string PestResistance { get; set; }
	public bool IsDroughtTolerant { get; set; }
	public bool IsFrostTolerant { get; set; }
	public bool IsHybrid { get; set; }
	public bool IsHeirloom { get; set; }
	public bool IsGMO { get; set; }
	public bool IsOrganic { get; set; }
	public bool IsTreated { get; set; }
	public string Certifications { get; set; }
	public DateTime? TestDate { get; set; }
	public string LotNumber { get; set; }
	public DateTime? PackageDate { get; set; }
	public DateTime? ExpiryDate { get; set; }
	public string StorageInstructions { get; set; }
	public DateTime? UpdatedAt { get; set; }
}
