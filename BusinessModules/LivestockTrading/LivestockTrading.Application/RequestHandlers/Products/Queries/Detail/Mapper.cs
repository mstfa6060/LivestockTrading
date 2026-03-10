using LivestockTrading.Domain.Entities;

namespace LivestockTrading.Application.RequestHandlers.Products.Queries.Detail;

public class Mapper
{
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
			CategoryName = entity.Category?.Name,
			BrandId = entity.BrandId,
			BrandName = entity.Brand?.Name,
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
			SellerName = entity.Seller?.BusinessName,
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
			ViewCount = entity.ViewCount,
			FavoriteCount = entity.FavoriteCount,
			AverageRating = entity.AverageRating,
			ReviewCount = entity.ReviewCount,
			PublishedAt = entity.PublishedAt,
			ExpiresAt = entity.ExpiresAt,
			CreatedAt = entity.CreatedAt,
			UpdatedAt = entity.UpdatedAt,
			MediaBucketId = entity.MediaBucketId,
			CoverImageFileId = entity.CoverImageFileId
		};
	}
}
