using LivestockTrading.Domain.Entities;

namespace LivestockTrading.Application.RequestHandlers.FeedInfos.Commands.Update;

public class Mapper
{
	public void MapToEntity(RequestModel request, FeedInfo entity)
	{
		entity.ProductId = request.ProductId;
		entity.Type = (FeedType)request.Type;
		entity.TargetAnimal = request.TargetAnimal;
		entity.TargetAge = request.TargetAge;
		entity.ProteinPercentage = request.ProteinPercentage;
		entity.FatPercentage = request.FatPercentage;
		entity.FiberPercentage = request.FiberPercentage;
		entity.MoisturePercentage = request.MoisturePercentage;
		entity.Form = (FeedForm)request.Form;
		entity.IsOrganic = request.IsOrganic;
		entity.IsGMOFree = request.IsGMOFree;
		entity.IsMedicatedFeed = request.IsMedicatedFeed;
		entity.FeedingInstructions = request.FeedingInstructions;
		entity.StorageInstructions = request.StorageInstructions;
		entity.ShelfLifeMonths = request.ShelfLifeMonths;
		entity.ExpiryDate = request.ExpiryDate;
		entity.BatchNumber = request.BatchNumber;
		entity.Certifications = request.Certifications;
		entity.UpdatedAt = DateTime.UtcNow;
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
			UpdatedAt = entity.UpdatedAt
		};
	}
}
