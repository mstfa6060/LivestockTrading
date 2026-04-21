using LivestockTrading.Domain.Entities;

namespace LivestockTrading.Application.RequestHandlers.SeedInfos.Queries.Detail;

public class Mapper
{
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
			CreatedAt = entity.CreatedAt,
			UpdatedAt = entity.UpdatedAt
		};
	}
}
