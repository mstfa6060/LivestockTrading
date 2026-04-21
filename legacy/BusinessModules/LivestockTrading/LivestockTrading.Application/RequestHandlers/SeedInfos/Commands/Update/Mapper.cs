using LivestockTrading.Domain.Entities;

namespace LivestockTrading.Application.RequestHandlers.SeedInfos.Commands.Update;

public class Mapper
{
	public void MapToEntity(RequestModel request, SeedInfo entity)
	{
		entity.ProductId = request.ProductId;
		entity.Type = (SeedType)request.Type;
		entity.Variety = request.Variety;
		entity.ScientificName = request.ScientificName;
		entity.CommonNames = request.CommonNames;
		entity.SeedSize = request.SeedSize;
		entity.SeedColor = request.SeedColor;
		entity.GerminationRate = request.GerminationRate;
		entity.GerminationDays = request.GerminationDays;
		entity.ClimateZones = request.ClimateZones;
		entity.SoilType = request.SoilType;
		entity.SunlightRequirement = request.SunlightRequirement;
		entity.WaterRequirement = request.WaterRequirement;
		entity.PlantingDepthCm = request.PlantingDepthCm;
		entity.SpacingCm = request.SpacingCm;
		entity.DaysToMaturity = request.DaysToMaturity;
		entity.PlantingSeason = request.PlantingSeason;
		entity.HarvestSeason = request.HarvestSeason;
		entity.ExpectedYield = request.ExpectedYield;
		entity.YieldUnit = request.YieldUnit;
		entity.PlantHeightCm = request.PlantHeightCm;
		entity.PlantSpreadCm = request.PlantSpreadCm;
		entity.FlowerColor = request.FlowerColor;
		entity.FruitSize = request.FruitSize;
		entity.DiseaseResistance = request.DiseaseResistance;
		entity.PestResistance = request.PestResistance;
		entity.IsDroughtTolerant = request.IsDroughtTolerant;
		entity.IsFrostTolerant = request.IsFrostTolerant;
		entity.IsHybrid = request.IsHybrid;
		entity.IsHeirloom = request.IsHeirloom;
		entity.IsGMO = request.IsGMO;
		entity.IsOrganic = request.IsOrganic;
		entity.IsTreated = request.IsTreated;
		entity.Certifications = request.Certifications;
		entity.TestDate = request.TestDate;
		entity.LotNumber = request.LotNumber;
		entity.PackageDate = request.PackageDate;
		entity.ExpiryDate = request.ExpiryDate;
		entity.StorageInstructions = request.StorageInstructions;
		entity.UpdatedAt = DateTime.UtcNow;
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
			UpdatedAt = entity.UpdatedAt
		};
	}
}
