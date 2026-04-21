using LivestockTrading.Domain.Entities;

namespace LivestockTrading.Application.RequestHandlers.ChemicalInfos.Queries.Detail;

public class Mapper
{
	public ResponseModel MapToResponse(ChemicalInfo entity)
	{
		return new ResponseModel
		{
			Id = entity.Id,
			ProductId = entity.ProductId,
			Type = (int)entity.Type,
			SubType = entity.SubType,
			ActiveIngredients = entity.ActiveIngredients,
			InertIngredients = entity.InertIngredients,
			ChemicalFormula = entity.ChemicalFormula,
			RegistrationNumber = entity.RegistrationNumber,
			ApprovalAgency = entity.ApprovalAgency,
			RegistrationDate = entity.RegistrationDate,
			ExpiryDate = entity.ExpiryDate,
			ApplicationMethod = entity.ApplicationMethod,
			TargetPests = entity.TargetPests,
			TargetCrops = entity.TargetCrops,
			DosageInstructions = entity.DosageInstructions,
			ToxicityLevel = (int)entity.ToxicityLevel,
			SafetyInstructions = entity.SafetyInstructions,
			FirstAidInstructions = entity.FirstAidInstructions,
			ReEntryIntervalHours = entity.ReEntryIntervalHours,
			PreHarvestIntervalDays = entity.PreHarvestIntervalDays,
			IsOrganic = entity.IsOrganic,
			IsBiodegradable = entity.IsBiodegradable,
			EnvironmentalImpact = entity.EnvironmentalImpact,
			StorageInstructions = entity.StorageInstructions,
			StorageTemperature = entity.StorageTemperature,
			ShelfLifeMonths = entity.ShelfLifeMonths,
			Certifications = entity.Certifications,
			CreatedAt = entity.CreatedAt,
			UpdatedAt = entity.UpdatedAt
		};
	}
}
