using LivestockTrading.Domain.Entities;

namespace LivestockTrading.Application.RequestHandlers.ProductImages.Commands.Create;

public class Mapper
{
	public ProductImage MapToEntity(RequestModel request)
	{
		return new ProductImage
		{
			Id = Guid.NewGuid(),
			ProductId = request.ProductId,
			ImageUrl = request.ImageUrl,
			ThumbnailUrl = request.ThumbnailUrl,
			AltText = request.AltText,
			SortOrder = request.SortOrder,
			IsPrimary = request.IsPrimary,
			CreatedAt = DateTime.UtcNow
		};
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
			CreatedAt = entity.CreatedAt
		};
	}
}
