using LivestockTrading.Domain.Entities;

namespace LivestockTrading.Application.RequestHandlers.Products.Queries.All;

public class Mapper
{
	public List<ResponseModel> MapToResponse(
		List<Product> products,
		string targetCurrencyCode = null,
		Dictionary<string, Currency> currencyRates = null)
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
				AverageRating = p.AverageRating,
				ReviewCount = p.ReviewCount,
				CreatedAt = p.CreatedAt,
				MediaBucketId = p.MediaBucketId,
				CoverImageFileId = p.CoverImageFileId
			};

			// Fiyat dönüşümü
			if (!string.IsNullOrWhiteSpace(targetCurrencyCode) && currencyRates != null
				&& !string.IsNullOrWhiteSpace(p.Currency)
				&& currencyRates.TryGetValue(p.Currency, out var fromCurrency)
				&& currencyRates.TryGetValue(targetCurrencyCode.ToUpperInvariant(), out var toCurrency)
				&& fromCurrency.ExchangeRateToUSD > 0)
			{
				var amountInUsd = p.BasePrice / fromCurrency.ExchangeRateToUSD;
				response.ConvertedPrice = Math.Round(amountInUsd * toCurrency.ExchangeRateToUSD, 2);
				response.ConvertedCurrencyCode = toCurrency.Code;
				response.ConvertedCurrencySymbol = toCurrency.Symbol;

				if (p.DiscountedPrice.HasValue)
				{
					var discountInUsd = p.DiscountedPrice.Value / fromCurrency.ExchangeRateToUSD;
					response.ConvertedDiscountedPrice = Math.Round(discountInUsd * toCurrency.ExchangeRateToUSD, 2);
				}
			}

			return response;
		}).ToList();
	}
}
