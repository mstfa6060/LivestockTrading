using LivestockTrading.Domain.Entities;

namespace LivestockTrading.Application.RequestHandlers.VeterinaryInfos.Queries.Detail;

public class Mapper
{
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
			CreatedAt = entity.CreatedAt,
			UpdatedAt = entity.UpdatedAt
		};
	}
}
