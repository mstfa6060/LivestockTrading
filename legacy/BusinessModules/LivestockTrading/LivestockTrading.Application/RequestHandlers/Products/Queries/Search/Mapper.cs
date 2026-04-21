using LivestockTrading.Domain.Entities;

namespace LivestockTrading.Application.RequestHandlers.Products.Queries.Search;

public class Mapper
{
	public List<ResponseModel> MapToResponse(
		List<Product> products,
		Dictionary<Guid, ProductPrice> viewerPrices = null,
		string viewerCurrencyCode = null,
		string viewerCurrencySymbol = null)
	{
		return products.Select(p =>
		{
			var response = new ResponseModel
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
				CreatedAt = p.CreatedAt
			};

			// Viewer currency (pre-computed ProductPrice based)
			if (viewerPrices != null && viewerPrices.TryGetValue(p.Id, out var vp))
			{
				response.ViewerPrice = vp.Price;
				response.ViewerDiscountedPrice = vp.DiscountedPrice;
				response.ViewerCurrencyCode = viewerCurrencyCode;
				response.ViewerCurrencySymbol = viewerCurrencySymbol;
			}

			return response;
		}).ToList();
	}
}
