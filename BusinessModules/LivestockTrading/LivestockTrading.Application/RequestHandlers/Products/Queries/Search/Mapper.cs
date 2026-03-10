using LivestockTrading.Domain.Entities;

namespace LivestockTrading.Application.RequestHandlers.Products.Queries.Search;

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
			FavoriteCount = p.FavoriteCount,
			AverageRating = p.AverageRating,
			ReviewCount = p.ReviewCount,
			CoverImageFileId = p.CoverImageFileId,
			MediaBucketId = p.MediaBucketId,
			CoverImageUrl = !string.IsNullOrWhiteSpace(p.CoverImageFileId) && imagePaths.TryGetValue(p.CoverImageFileId, out var path) ? path : null,
			CreatedAt = p.CreatedAt
		}).ToList();
	}
}
