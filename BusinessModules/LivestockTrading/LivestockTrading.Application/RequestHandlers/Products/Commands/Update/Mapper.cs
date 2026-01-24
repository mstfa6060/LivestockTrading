using LivestockTrading.Domain.Entities;

namespace LivestockTrading.Application.RequestHandlers.Products.Commands.Update;

public class Mapper
{
	public void MapToEntity(RequestModel request, Product entity)
	{
		entity.Title = request.Title;
		entity.Slug = request.Slug;
		entity.Description = request.Description;
		entity.ShortDescription = request.ShortDescription;
		entity.CategoryId = request.CategoryId;
		entity.BrandId = request.BrandId;
		entity.BasePrice = request.BasePrice;
		entity.Currency = request.Currency;
		entity.DiscountedPrice = request.DiscountedPrice;
		entity.PriceUnit = request.PriceUnit;
		entity.StockQuantity = request.StockQuantity;
		entity.StockUnit = request.StockUnit;
		entity.MinOrderQuantity = request.MinOrderQuantity;
		entity.MaxOrderQuantity = request.MaxOrderQuantity;
		entity.IsInStock = request.IsInStock;
		entity.SellerId = request.SellerId;
		entity.LocationId = request.LocationId;
		entity.Status = (ProductStatus)request.Status;
		entity.Condition = (ProductCondition)request.Condition;
		entity.IsShippingAvailable = request.IsShippingAvailable;
		entity.ShippingCost = request.ShippingCost;
		entity.IsInternationalShipping = request.IsInternationalShipping;
		entity.Weight = request.Weight;
		entity.WeightUnit = request.WeightUnit;
		entity.Attributes = request.Attributes;
		entity.MetaTitle = request.MetaTitle;
		entity.MetaDescription = request.MetaDescription;
		entity.MetaKeywords = request.MetaKeywords;
		entity.UpdatedAt = DateTime.UtcNow;
	}

	public ResponseModel MapToResponse(Product entity)
	{
		return new ResponseModel
		{
			Id = entity.Id,
			Title = entity.Title,
			Slug = entity.Slug,
			Description = entity.Description,
			ShortDescription = entity.ShortDescription,
			CategoryId = entity.CategoryId,
			BrandId = entity.BrandId,
			BasePrice = entity.BasePrice,
			Currency = entity.Currency,
			DiscountedPrice = entity.DiscountedPrice,
			PriceUnit = entity.PriceUnit,
			StockQuantity = entity.StockQuantity,
			StockUnit = entity.StockUnit,
			MinOrderQuantity = entity.MinOrderQuantity,
			MaxOrderQuantity = entity.MaxOrderQuantity,
			IsInStock = entity.IsInStock,
			SellerId = entity.SellerId,
			LocationId = entity.LocationId,
			Status = (int)entity.Status,
			Condition = (int)entity.Condition,
			IsShippingAvailable = entity.IsShippingAvailable,
			ShippingCost = entity.ShippingCost,
			IsInternationalShipping = entity.IsInternationalShipping,
			Weight = entity.Weight,
			WeightUnit = entity.WeightUnit,
			Attributes = entity.Attributes,
			MetaTitle = entity.MetaTitle,
			MetaDescription = entity.MetaDescription,
			MetaKeywords = entity.MetaKeywords,
			UpdatedAt = entity.UpdatedAt
		};
	}
}
