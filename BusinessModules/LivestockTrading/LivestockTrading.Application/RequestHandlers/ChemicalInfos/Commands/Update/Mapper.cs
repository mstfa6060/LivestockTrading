using LivestockTrading.Domain.Entities;

namespace LivestockTrading.Application.RequestHandlers.ChemicalInfos.Commands.Update;

public class Mapper
{
	public void MapToEntity(RequestModel request, ChemicalInfo entity)
	{
		entity.ProductId = request.ProductId;
		entity.Type = (ChemicalType)request.Type;
		entity.SubType = request.SubType;
		entity.ActiveIngredients = request.ActiveIngredients;
		entity.InertIngredients = request.InertIngredients;
		entity.ChemicalFormula = request.ChemicalFormula;
		entity.RegistrationNumber = request.RegistrationNumber;
		entity.ApprovalAgency = request.ApprovalAgency;
		entity.RegistrationDate = request.RegistrationDate;
		entity.ExpiryDate = request.ExpiryDate;
		entity.ApplicationMethod = request.ApplicationMethod;
		entity.TargetPests = request.TargetPests;
		entity.TargetCrops = request.TargetCrops;
		entity.DosageInstructions = request.DosageInstructions;
		entity.ToxicityLevel = (ToxicityLevel)request.ToxicityLevel;
		entity.SafetyInstructions = request.SafetyInstructions;
		entity.FirstAidInstructions = request.FirstAidInstructions;
		entity.ReEntryIntervalHours = request.ReEntryIntervalHours;
		entity.PreHarvestIntervalDays = request.PreHarvestIntervalDays;
		entity.IsOrganic = request.IsOrganic;
		entity.IsBiodegradable = request.IsBiodegradable;
		entity.EnvironmentalImpact = request.EnvironmentalImpact;
		entity.StorageInstructions = request.StorageInstructions;
		entity.StorageTemperature = request.StorageTemperature;
		entity.ShelfLifeMonths = request.ShelfLifeMonths;
		entity.Certifications = request.Certifications;
		entity.UpdatedAt = DateTime.UtcNow;
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
			UpdatedAt = entity.UpdatedAt
		};
	}
}
