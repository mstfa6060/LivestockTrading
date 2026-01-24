using LivestockTrading.Domain.Entities;

namespace LivestockTrading.Application.RequestHandlers.ProductVariants.Commands.Create;

public class Mapper
{
	public ProductVariant MapToEntity(RequestModel request)
	{
		return new ProductVariant
		{
			Id = Guid.NewGuid(),
			ProductId = request.ProductId,
			Name = request.Name,
			SKU = request.SKU,
			Price = request.Price,
			DiscountedPrice = request.DiscountedPrice,
			StockQuantity = request.StockQuantity,
			IsInStock = request.IsInStock,
			Attributes = request.Attributes,
			ImageUrl = request.ImageUrl,
			IsActive = request.IsActive,
			SortOrder = request.SortOrder,
			CreatedAt = DateTime.UtcNow
		};
	}

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
			CreatedAt = entity.CreatedAt
		};
	}
}
