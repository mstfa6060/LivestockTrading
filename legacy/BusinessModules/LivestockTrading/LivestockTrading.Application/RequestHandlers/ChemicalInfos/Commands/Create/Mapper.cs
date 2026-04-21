using LivestockTrading.Domain.Entities;

namespace LivestockTrading.Application.RequestHandlers.ChemicalInfos.Commands.Create;

public class Mapper
{
	public ChemicalInfo MapToEntity(RequestModel request)
	{
		return new ChemicalInfo
		{
			Id = Guid.NewGuid(),
			ProductId = request.ProductId,
			Type = (ChemicalType)request.Type,
			SubType = request.SubType,
			ActiveIngredients = request.ActiveIngredients,
			InertIngredients = request.InertIngredients,
			ChemicalFormula = request.ChemicalFormula,
			RegistrationNumber = request.RegistrationNumber,
			ApprovalAgency = request.ApprovalAgency,
			RegistrationDate = request.RegistrationDate,
			ExpiryDate = request.ExpiryDate,
			ApplicationMethod = request.ApplicationMethod,
			TargetPests = request.TargetPests,
			TargetCrops = request.TargetCrops,
			DosageInstructions = request.DosageInstructions,
			ToxicityLevel = (ToxicityLevel)request.ToxicityLevel,
			SafetyInstructions = request.SafetyInstructions,
			FirstAidInstructions = request.FirstAidInstructions,
			ReEntryIntervalHours = request.ReEntryIntervalHours,
			PreHarvestIntervalDays = request.PreHarvestIntervalDays,
			IsOrganic = request.IsOrganic,
			IsBiodegradable = request.IsBiodegradable,
			EnvironmentalImpact = request.EnvironmentalImpact,
			StorageInstructions = request.StorageInstructions,
			StorageTemperature = request.StorageTemperature,
			ShelfLifeMonths = request.ShelfLifeMonths,
			Certifications = request.Certifications,
			CreatedAt = DateTime.UtcNow
		};
	}

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
			CreatedAt = entity.CreatedAt
		};
	}
}
