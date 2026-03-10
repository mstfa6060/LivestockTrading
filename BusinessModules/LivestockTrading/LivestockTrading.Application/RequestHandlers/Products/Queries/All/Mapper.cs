using LivestockTrading.Domain.Entities;

namespace LivestockTrading.Application.RequestHandlers.Products.Queries.All;

public class Mapper
{
	public List<ResponseModel> MapToResponse(List<Product> products, Dictionary<string, string> imagePaths)
	{
		return products.Select(p => new ResponseModel
		{
			Id = p.Id,
			Title = p.Title,
			Slug = p.Slug,
			ShortDescription = p.ShortDescription,
			CategoryId = p.CategoryId,
			BrandId = p.BrandId,
			BasePrice = p.BasePrice,
			Currency = p.Currency,
			DiscountedPrice = p.DiscountedPrice,
			StockQuantity = p.StockQuantity,
			IsInStock = p.IsInStock,
			SellerId = p.SellerId,
			LocationId = p.LocationId,
			LocationCountryCode = p.Location?.CountryCode,
			LocationCity = p.Location?.City,
			Status = (int)p.Status,
			Condition = (int)p.Condition,
			ViewCount = p.ViewCount,
			AverageRating = p.AverageRating,
			ReviewCount = p.ReviewCount,
			CreatedAt = p.CreatedAt,
			MediaBucketId = p.MediaBucketId,
			CoverImageUrl = !string.IsNullOrWhiteSpace(p.CoverImageFileId) && imagePaths.TryGetValue(p.CoverImageFileId, out var path) ? path : null
		}).ToList();
	}
}
