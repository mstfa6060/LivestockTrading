using LivestockTrading.Domain.Entities;

namespace LivestockTrading.Application.RequestHandlers.FeedInfos.Commands.Create;

public class Mapper
{
	public FeedInfo MapToEntity(RequestModel request)
	{
		return new FeedInfo
		{
			Id = Guid.NewGuid(),
			ProductId = request.ProductId,
			Type = (FeedType)request.Type,
			TargetAnimal = request.TargetAnimal,
			TargetAge = request.TargetAge,
			ProteinPercentage = request.ProteinPercentage,
			FatPercentage = request.FatPercentage,
			FiberPercentage = request.FiberPercentage,
			MoisturePercentage = request.MoisturePercentage,
			Form = (FeedForm)request.Form,
			IsOrganic = request.IsOrganic,
			IsGMOFree = request.IsGMOFree,
			IsMedicatedFeed = request.IsMedicatedFeed,
			FeedingInstructions = request.FeedingInstructions,
			StorageInstructions = request.StorageInstructions,
			ShelfLifeMonths = request.ShelfLifeMonths,
			ExpiryDate = request.ExpiryDate,
			BatchNumber = request.BatchNumber,
			Certifications = request.Certifications,
			CreatedAt = DateTime.UtcNow
		};
	}

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
			CreatedAt = entity.CreatedAt
		};
	}
}
