using LivestockTrading.Domain.Entities;

namespace LivestockTrading.Application.RequestHandlers.VeterinaryInfos.Commands.Update;

public class Mapper
{
	public void MapToEntity(RequestModel request, VeterinaryInfo entity)
	{
		entity.ProductId = request.ProductId;
		entity.Type = (VeterinaryProductType)request.Type;
		entity.TherapeuticCategory = request.TherapeuticCategory;
		entity.TargetSpecies = request.TargetSpecies;
		entity.Indications = request.Indications;
		entity.ActiveIngredients = request.ActiveIngredients;
		entity.Strength = request.Strength;
		entity.Route = (AdministrationRoute)request.Route;
		entity.DosageInstructions = request.DosageInstructions;
		entity.RequiresPrescription = request.RequiresPrescription;
		entity.RegistrationNumber = request.RegistrationNumber;
		entity.Contraindications = request.Contraindications;
		entity.MeatWithdrawalDays = request.MeatWithdrawalDays;
		entity.MilkWithdrawalDays = request.MilkWithdrawalDays;
		entity.EggWithdrawalDays = request.EggWithdrawalDays;
		entity.StorageInstructions = request.StorageInstructions;
		entity.ShelfLifeMonths = request.ShelfLifeMonths;
		entity.ExpiryDate = request.ExpiryDate;
		entity.BatchNumber = request.BatchNumber;
		entity.RequiresColdChain = request.RequiresColdChain;
		entity.Certifications = request.Certifications;
		entity.UpdatedAt = DateTime.UtcNow;
	}

	public ResponseModel MapToResponse(VeterinaryInfo entity)
	{
		return new ResponseModel
		{
			Id = entity.Id,
			ProductId = entity.ProductId,
			Type = (int)entity.Type,
			TherapeuticCategory = entity.TherapeuticCategory,
			TargetSpecies = entity.TargetSpecies,
			Indications = entity.Indications,
			ActiveIngredients = entity.ActiveIngredients,
			Strength = entity.Strength,
			Route = (int)entity.Route,
			DosageInstructions = entity.DosageInstructions,
			RequiresPrescription = entity.RequiresPrescription,
			RegistrationNumber = entity.RegistrationNumber,
			Contraindications = entity.Contraindications,
			MeatWithdrawalDays = entity.MeatWithdrawalDays,
			MilkWithdrawalDays = entity.MilkWithdrawalDays,
			EggWithdrawalDays = entity.EggWithdrawalDays,
			StorageInstructions = entity.StorageInstructions,
			ShelfLifeMonths = entity.ShelfLifeMonths,
			ExpiryDate = entity.ExpiryDate,
			BatchNumber = entity.BatchNumber,
			RequiresColdChain = entity.RequiresColdChain,
			Certifications = entity.Certifications,
			UpdatedAt = entity.UpdatedAt
		};
	}
}
