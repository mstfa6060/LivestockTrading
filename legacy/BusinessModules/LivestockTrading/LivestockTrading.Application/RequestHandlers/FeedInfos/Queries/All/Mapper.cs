using LivestockTrading.Domain.Entities;

namespace LivestockTrading.Application.RequestHandlers.FeedInfos.Queries.All;

public class Mapper
{
	public List<ResponseModel> MapToResponse(List<FeedInfo> items)
	{
		return items.Select(e => new ResponseModel
		{
			Id = e.Id,
			ProductId = e.ProductId,
			Type = (int)e.Type,
			TargetAnimal = e.TargetAnimal,
			TargetAge = e.TargetAge,
			ProteinPercentage = e.ProteinPercentage,
			FatPercentage = e.FatPercentage,
			FiberPercentage = e.FiberPercentage,
			MoisturePercentage = e.MoisturePercentage,
			Form = (int)e.Form,
			IsOrganic = e.IsOrganic,
			IsGMOFree = e.IsGMOFree,
			IsMedicatedFeed = e.IsMedicatedFeed,
			FeedingInstructions = e.FeedingInstructions,
			StorageInstructions = e.StorageInstructions,
			ShelfLifeMonths = e.ShelfLifeMonths,
			ExpiryDate = e.ExpiryDate,
			BatchNumber = e.BatchNumber,
			Certifications = e.Certifications,
			CreatedAt = e.CreatedAt
		}).ToList();
	}
}
