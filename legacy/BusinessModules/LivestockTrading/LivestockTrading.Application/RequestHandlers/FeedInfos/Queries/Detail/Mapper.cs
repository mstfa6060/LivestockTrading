using LivestockTrading.Domain.Entities;

namespace LivestockTrading.Application.RequestHandlers.FeedInfos.Queries.Detail;

public class Mapper
{
	public ResponseModel MapToResponse(FeedInfo entity)
	{
		return new ResponseModel
		{
			Id = entity.Id,
			ProductId = entity.ProductId,
			Type = (int)entity.Type,
			TargetAnimal = entity.TargetAnimal,
			TargetAge = entity.TargetAge,
			ProteinPercentage = entity.ProteinPercentage,
			FatPercentage = entity.FatPercentage,
			FiberPercentage = entity.FiberPercentage,
			MoisturePercentage = entity.MoisturePercentage,
			Form = (int)entity.Form,
			IsOrganic = entity.IsOrganic,
			IsGMOFree = entity.IsGMOFree,
			IsMedicatedFeed = entity.IsMedicatedFeed,
			FeedingInstructions = entity.FeedingInstructions,
			StorageInstructions = entity.StorageInstructions,
			ShelfLifeMonths = entity.ShelfLifeMonths,
			ExpiryDate = entity.ExpiryDate,
			BatchNumber = entity.BatchNumber,
			Certifications = entity.Certifications,
			CreatedAt = entity.CreatedAt,
			UpdatedAt = entity.UpdatedAt
		};
	}
}
