using LivestockTrading.Domain.Entities;


namespace LivestockTrading.Application.RequestHandlers.Banners.Commands.Create;

public class Mapper
{
	public Banner MapToEntity(RequestModel request)
	{
		return new Banner
		{
			Id = Guid.NewGuid(),
			Title = request.Title,
			Description = request.Description,
			ImageUrl = request.ImageUrl,
			TargetUrl = request.TargetUrl,
			Position = (BannerPosition)request.Position,
			StartDate = request.StartDate,
			EndDate = request.EndDate,
			IsActive = request.IsActive,
			DisplayOrder = request.DisplayOrder,
			ClickCount = 0,
			ImpressionCount = 0,
			CreatedAt = DateTime.UtcNow
		};
	}

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
			CreatedAt = entity.CreatedAt
		};
	}
}
