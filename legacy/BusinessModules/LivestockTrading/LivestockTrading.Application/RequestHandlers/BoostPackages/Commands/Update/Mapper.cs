using LivestockTrading.Domain.Entities;

namespace LivestockTrading.Application.RequestHandlers.BoostPackages.Commands.Update;

public class Mapper
{
	public void MapToEntity(RequestModel request, BoostPackage entity)
	{
		entity.Name = request.Name;
		entity.Description = request.Description;
		entity.NameTranslations = request.NameTranslations;
		entity.DescriptionTranslations = request.DescriptionTranslations;
		entity.DurationHours = request.DurationHours;
		entity.Price = request.Price;
		entity.Currency = request.Currency;
		entity.BoostType = (BoostType)request.BoostType;
		entity.BoostScore = request.BoostScore;
		entity.AppleProductId = request.AppleProductId;
		entity.GoogleProductId = request.GoogleProductId;
		entity.IsActive = request.IsActive;
		entity.SortOrder = request.SortOrder;
		entity.UpdatedAt = DateTime.UtcNow;
	}

	public ResponseModel MapToResponse(BoostPackage entity)
	{
		return new ResponseModel
		{
			Id = entity.Id,
			Name = entity.Name,
			Description = entity.Description,
			NameTranslations = entity.NameTranslations,
			DescriptionTranslations = entity.DescriptionTranslations,
			DurationHours = entity.DurationHours,
			Price = entity.Price,
			Currency = entity.Currency,
			BoostType = (int)entity.BoostType,
			BoostScore = entity.BoostScore,
			AppleProductId = entity.AppleProductId,
			GoogleProductId = entity.GoogleProductId,
			IsActive = entity.IsActive,
			SortOrder = entity.SortOrder,
			CreatedAt = entity.CreatedAt,
			UpdatedAt = entity.UpdatedAt
		};
	}
}
