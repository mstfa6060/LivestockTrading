using LivestockTrading.Domain.Entities;

namespace LivestockTrading.Application.RequestHandlers.Products.Commands.Create;

public class Mapper
{
	public Product MapToEntity(RequestModel request)
	{
		return new Product
		{
			Id = Guid.NewGuid(),
			Title = request.Title,
			Slug = request.Slug,
			Description = request.Description,
			ShortDescription = request.ShortDescription,
			CategoryId = request.CategoryId,
			BrandId = request.BrandId,
			BasePrice = request.BasePrice,
			Currency = request.Currency,
			DiscountedPrice = request.DiscountedPrice,
			PriceUnit = request.PriceUnit,
			StockQuantity = request.StockQuantity,
			StockUnit = request.StockUnit,
			MinOrderQuantity = request.MinOrderQuantity,
			MaxOrderQuantity = request.MaxOrderQuantity,
			IsInStock = request.IsInStock,
			// SellerId Handler'da set edilir (otomatik oluşturma mantığı)
			LocationId = request.LocationId,
			Status = ProductStatus.PendingApproval,
			Condition = (ProductCondition)request.Condition,
			IsShippingAvailable = request.IsShippingAvailable,
			ShippingCost = request.ShippingCost,
			IsInternationalShipping = request.IsInternationalShipping,
			Weight = request.Weight,
			WeightUnit = request.WeightUnit,
			Attributes = request.Attributes,
			MetaTitle = request.MetaTitle,
			MetaDescription = request.MetaDescription,
			MetaKeywords = request.MetaKeywords,
			MediaBucketId = request.MediaBucketId,
			CoverImageFileId = request.CoverImageFileId,
			CreatedAt = DateTime.UtcNow
		};
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
			MediaBucketId = entity.MediaBucketId,
			CoverImageFileId = entity.CoverImageFileId,
			CreatedAt = entity.CreatedAt
		};
	}
}
