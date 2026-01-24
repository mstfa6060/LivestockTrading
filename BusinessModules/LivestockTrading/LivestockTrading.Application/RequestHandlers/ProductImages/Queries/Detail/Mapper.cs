using LivestockTrading.Domain.Entities;

namespace LivestockTrading.Application.RequestHandlers.ProductImages.Queries.Detail;

public class Mapper
{
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
			CreatedAt = entity.CreatedAt,
			UpdatedAt = entity.UpdatedAt
		};
	}
}
