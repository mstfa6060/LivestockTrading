using LivestockTrading.Domain.Entities;

namespace LivestockTrading.Application.RequestHandlers.ProductVariants.Queries.All;

public class Mapper
{
	public List<ResponseModel> MapToResponse(List<ProductVariant> items)
	{
		return items.Select(e => new ResponseModel
		{
			Id = e.Id,
			ProductId = e.ProductId,
			Name = e.Name,
			SKU = e.SKU,
			Price = e.Price,
			DiscountedPrice = e.DiscountedPrice,
			StockQuantity = e.StockQuantity,
			IsInStock = e.IsInStock,
			Attributes = e.Attributes,
			ImageUrl = e.ImageUrl,
			IsActive = e.IsActive,
			SortOrder = e.SortOrder,
			CreatedAt = e.CreatedAt
		}).ToList();
	}
}
