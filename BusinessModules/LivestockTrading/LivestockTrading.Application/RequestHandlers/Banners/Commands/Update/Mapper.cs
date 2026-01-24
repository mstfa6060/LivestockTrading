using LivestockTrading.Domain.Entities;
using LivestockTrading.Domain.Enums;

namespace LivestockTrading.Application.RequestHandlers.Banners.Commands.Update;

public class Mapper
{
	public void MapToEntity(RequestModel request, Banner entity)
	{
		entity.Title = request.Title;
		entity.Description = request.Description;
		entity.ImageUrl = request.ImageUrl;
		entity.TargetUrl = request.TargetUrl;
		entity.Position = (BannerPosition)request.Position;
		entity.StartDate = request.StartDate;
		entity.EndDate = request.EndDate;
		entity.IsActive = request.IsActive;
		entity.DisplayOrder = request.DisplayOrder;
		entity.UpdatedAt = DateTime.UtcNow;
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
			UpdatedAt = entity.UpdatedAt
		};
	}
}
