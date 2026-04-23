using LivestockTrading.Domain.Entities;

namespace LivestockTrading.Application.RequestHandlers.VeterinaryInfos.Queries.All;

public class Mapper
{
	public List<ResponseModel> MapToResponse(List<VeterinaryInfo> items)
	{
		return items.Select(e => new ResponseModel
		{
			Id = e.Id,
			ProductId = e.ProductId,
			Type = (int)e.Type,
			TherapeuticCategory = e.TherapeuticCategory,
			TargetSpecies = e.TargetSpecies,
			Indications = e.Indications,
			ActiveIngredients = e.ActiveIngredients,
			Strength = e.Strength,
			Route = (int)e.Route,
			DosageInstructions = e.DosageInstructions,
			RequiresPrescription = e.RequiresPrescription,
			RegistrationNumber = e.RegistrationNumber,
			Contraindications = e.Contraindications,
			MeatWithdrawalDays = e.MeatWithdrawalDays,
			MilkWithdrawalDays = e.MilkWithdrawalDays,
			EggWithdrawalDays = e.EggWithdrawalDays,
			StorageInstructions = e.StorageInstructions,
			ShelfLifeMonths = e.ShelfLifeMonths,
			ExpiryDate = e.ExpiryDate,
			BatchNumber = e.BatchNumber,
			RequiresColdChain = e.RequiresColdChain,
			Certifications = e.Certifications,
			CreatedAt = e.CreatedAt
		}).ToList();
	}
}
