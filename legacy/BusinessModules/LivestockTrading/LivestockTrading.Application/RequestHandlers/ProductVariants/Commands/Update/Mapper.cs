using LivestockTrading.Domain.Entities;

namespace LivestockTrading.Application.RequestHandlers.ProductVariants.Commands.Update;

public class Mapper
{
	public void MapToEntity(RequestModel request, ProductVariant entity)
	{
		entity.ProductId = request.ProductId;
		entity.Name = request.Name;
		entity.SKU = request.SKU;
		entity.Price = request.Price;
		entity.DiscountedPrice = request.DiscountedPrice;
		entity.StockQuantity = request.StockQuantity;
		entity.IsInStock = request.IsInStock;
		entity.Attributes = request.Attributes;
		entity.ImageUrl = request.ImageUrl;
		entity.IsActive = request.IsActive;
		entity.SortOrder = request.SortOrder;
		entity.UpdatedAt = DateTime.UtcNow;
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
			UpdatedAt = entity.UpdatedAt
		};
	}
}
