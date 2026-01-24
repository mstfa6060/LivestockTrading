using LivestockTrading.Domain.Entities;

namespace LivestockTrading.Application.RequestHandlers.ProductImages.Queries.All;

public class Mapper
{
	public List<ResponseModel> MapToResponse(List<ProductImage> items)
	{
		return items.Select(e => new ResponseModel
		{
			Id = e.Id,
			ProductId = e.ProductId,
			ImageUrl = e.ImageUrl,
			ThumbnailUrl = e.ThumbnailUrl,
			AltText = e.AltText,
			SortOrder = e.SortOrder,
			IsPrimary = e.IsPrimary,
			CreatedAt = e.CreatedAt
		}).ToList();
	}
}
