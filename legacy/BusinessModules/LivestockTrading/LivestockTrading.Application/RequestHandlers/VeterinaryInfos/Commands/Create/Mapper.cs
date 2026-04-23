using LivestockTrading.Domain.Entities;

namespace LivestockTrading.Application.RequestHandlers.VeterinaryInfos.Commands.Create;

public class Mapper
{
	public VeterinaryInfo MapToEntity(RequestModel request)
	{
		return new VeterinaryInfo
		{
			Id = Guid.NewGuid(),
			ProductId = request.ProductId,
			Type = (VeterinaryProductType)request.Type,
			TherapeuticCategory = request.TherapeuticCategory,
			TargetSpecies = request.TargetSpecies,
			Indications = request.Indications,
			ActiveIngredients = request.ActiveIngredients,
			Strength = request.Strength,
			Route = (AdministrationRoute)request.Route,
			DosageInstructions = request.DosageInstructions,
			RequiresPrescription = request.RequiresPrescription,
			RegistrationNumber = request.RegistrationNumber,
			Contraindications = request.Contraindications,
			MeatWithdrawalDays = request.MeatWithdrawalDays,
			MilkWithdrawalDays = request.MilkWithdrawalDays,
			EggWithdrawalDays = request.EggWithdrawalDays,
			StorageInstructions = request.StorageInstructions,
			ShelfLifeMonths = request.ShelfLifeMonths,
			ExpiryDate = request.ExpiryDate,
			BatchNumber = request.BatchNumber,
			RequiresColdChain = request.RequiresColdChain,
			Certifications = request.Certifications,
			CreatedAt = DateTime.UtcNow
		};
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
			CreatedAt = entity.CreatedAt
		};
	}
}
