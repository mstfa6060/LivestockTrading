using LivestockTrading.Domain.Entities;

namespace LivestockTrading.Application.RequestHandlers.Banners.Queries.Detail;

public class Mapper
{
	public ResponseModel MapToResponse(Banner entity)
	{
		return new ResponseModel
		{
			Id = entity.Id,
			Title = entity.Title,
			Description = entity.Description,
			ImageUrl = entity.ImageUrl,
			TargetUrl = entity.TargetUrl,
			Position = (int)entity.Position,
			StartDate = entity.StartDate,
			EndDate = entity.EndDate,
			IsActive = entity.IsActive,
			DisplayOrder = entity.DisplayOrder,
			ClickCount = entity.ClickCount,
			ImpressionCount = entity.ImpressionCount,
			CreatedAt = entity.CreatedAt,
			UpdatedAt = entity.UpdatedAt
		};
	}
}
