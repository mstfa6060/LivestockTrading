using LivestockTrading.Domain.Entities;

namespace LivestockTrading.Application.RequestHandlers.SeedInfos.Commands.Create;

public class Mapper
{
	public SeedInfo MapToEntity(RequestModel request)
	{
		return new SeedInfo
		{
			Id = Guid.NewGuid(),
			ProductId = request.ProductId,
			Type = (SeedType)request.Type,
			Variety = request.Variety,
			ScientificName = request.ScientificName,
			CommonNames = request.CommonNames,
			SeedSize = request.SeedSize,
			SeedColor = request.SeedColor,
			GerminationRate = request.GerminationRate,
			GerminationDays = request.GerminationDays,
			ClimateZones = request.ClimateZones,
			SoilType = request.SoilType,
			SunlightRequirement = request.SunlightRequirement,
			WaterRequirement = request.WaterRequirement,
			PlantingDepthCm = request.PlantingDepthCm,
			SpacingCm = request.SpacingCm,
			DaysToMaturity = request.DaysToMaturity,
			PlantingSeason = request.PlantingSeason,
			HarvestSeason = request.HarvestSeason,
			ExpectedYield = request.ExpectedYield,
			YieldUnit = request.YieldUnit,
			PlantHeightCm = request.PlantHeightCm,
			PlantSpreadCm = request.PlantSpreadCm,
			FlowerColor = request.FlowerColor,
			FruitSize = request.FruitSize,
			DiseaseResistance = request.DiseaseResistance,
			PestResistance = request.PestResistance,
			IsDroughtTolerant = request.IsDroughtTolerant,
			IsFrostTolerant = request.IsFrostTolerant,
			IsHybrid = request.IsHybrid,
			IsHeirloom = request.IsHeirloom,
			IsGMO = request.IsGMO,
			IsOrganic = request.IsOrganic,
			IsTreated = request.IsTreated,
			Certifications = request.Certifications,
			TestDate = request.TestDate,
			LotNumber = request.LotNumber,
			PackageDate = request.PackageDate,
			ExpiryDate = request.ExpiryDate,
			StorageInstructions = request.StorageInstructions,
			CreatedAt = DateTime.UtcNow
		};
	}

	public ResponseModel MapToResponse(SeedInfo entity)
	{
		return new ResponseModel
		{
			Id = entity.Id,
			ProductId = entity.ProductId,
			Type = (int)entity.Type,
			Variety = entity.Variety,
			ScientificName = entity.ScientificName,
			CommonNames = entity.CommonNames,
			SeedSize = entity.SeedSize,
			SeedColor = entity.SeedColor,
			GerminationRate = entity.GerminationRate,
			GerminationDays = entity.GerminationDays,
			ClimateZones = entity.ClimateZones,
			SoilType = entity.SoilType,
			SunlightRequirement = entity.SunlightRequirement,
			WaterRequirement = entity.WaterRequirement,
			PlantingDepthCm = entity.PlantingDepthCm,
			SpacingCm = entity.SpacingCm,
			DaysToMaturity = entity.DaysToMaturity,
			PlantingSeason = entity.PlantingSeason,
			HarvestSeason = entity.HarvestSeason,
			ExpectedYield = entity.ExpectedYield,
			YieldUnit = entity.YieldUnit,
			PlantHeightCm = entity.PlantHeightCm,
			PlantSpreadCm = entity.PlantSpreadCm,
			FlowerColor = entity.FlowerColor,
			FruitSize = entity.FruitSize,
			DiseaseResistance = entity.DiseaseResistance,
			PestResistance = entity.PestResistance,
			IsDroughtTolerant = entity.IsDroughtTolerant,
			IsFrostTolerant = entity.IsFrostTolerant,
			IsHybrid = entity.IsHybrid,
			IsHeirloom = entity.IsHeirloom,
			IsGMO = entity.IsGMO,
			IsOrganic = entity.IsOrganic,
			IsTreated = entity.IsTreated,
			Certifications = entity.Certifications,
			TestDate = entity.TestDate,
			LotNumber = entity.LotNumber,
			PackageDate = entity.PackageDate,
			ExpiryDate = entity.ExpiryDate,
			StorageInstructions = entity.StorageInstructions,
			CreatedAt = entity.CreatedAt
		};
	}
}
