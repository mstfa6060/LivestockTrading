using LivestockTrading.Domain.Entities;

namespace LivestockTrading.Application.RequestHandlers.ProductPrices.Queries.All;

public class Mapper
{
	public List<ResponseModel> MapToResponse(List<ProductPrice> items)
	{
		return items.Select(e => new ResponseModel
		{
			Id = e.Id,
			ProductId = e.ProductId,
			CurrencyCode = e.CurrencyCode,
			Price = e.Price,
			DiscountedPrice = e.DiscountedPrice,
			CountryCodes = e.CountryCodes,
			IsActive = e.IsActive,
			ValidFrom = e.ValidFrom,
			ValidUntil = e.ValidUntil,
			IsAutomaticConversion = e.IsAutomaticConversion,
			CreatedAt = e.CreatedAt
		}).ToList();
	}
}
