using LivestockTrading.Domain.Entities;

namespace LivestockTrading.Application.RequestHandlers.ProductImages.Commands.Update;

public class Mapper
{
	public void MapToEntity(RequestModel request, ProductImage entity)
	{
		entity.ImageUrl = request.ImageUrl;
		entity.ThumbnailUrl = request.ThumbnailUrl;
		entity.AltText = request.AltText;
		entity.SortOrder = request.SortOrder;
		entity.IsPrimary = request.IsPrimary;
		entity.UpdatedAt = DateTime.UtcNow;
	}

	public ResponseModel MapToResponse(ProductImage entity)
	{
		return new ResponseModel
		{
			Id = entity.Id,
			ProductId = entity.ProductId,
			ImageUrl = entity.ImageUrl,
			ThumbnailUrl = entity.ThumbnailUrl,
			AltText = entity.AltText,
			SortOrder = entity.SortOrder,
			IsPrimary = entity.IsPrimary,
			UpdatedAt = entity.UpdatedAt
		};
	}
}
