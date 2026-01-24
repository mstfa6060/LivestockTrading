using LivestockTrading.Domain.Entities;

namespace LivestockTrading.Application.RequestHandlers.ProductVariants.Queries.Detail;

public class Mapper
{
	public ResponseModel MapToResponse(ProductVariant entity)
	{
		return new ResponseModel
		{
			Id = entity.Id,
			ProductId = entity.ProductId,
			Name = entity.Name,
			SKU = entity.SKU,
			Price = entity.Price,
			DiscountedPrice = entity.DiscountedPrice,
			StockQuantity = entity.StockQuantity,
			IsInStock = entity.IsInStock,
			Attributes = entity.Attributes,
			ImageUrl = entity.ImageUrl,
			IsActive = entity.IsActive,
			SortOrder = entity.SortOrder,
			CreatedAt = entity.CreatedAt,
			UpdatedAt = entity.UpdatedAt
		};
	}
}
