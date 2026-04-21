using LivestockTrading.Domain.Entities;

namespace LivestockTrading.Application.RequestHandlers.ProductPrices.Queries.Detail;

public class Mapper
{
	public ResponseModel MapToResponse(ProductPrice entity)
	{
		return new ResponseModel
		{
			Id = entity.Id,
			ProductId = entity.ProductId,
			CurrencyCode = entity.CurrencyCode,
			Price = entity.Price,
			DiscountedPrice = entity.DiscountedPrice,
			CountryCodes = entity.CountryCodes,
			IsActive = entity.IsActive,
			ValidFrom = entity.ValidFrom,
			ValidUntil = entity.ValidUntil,
			IsAutomaticConversion = entity.IsAutomaticConversion,
			CreatedAt = entity.CreatedAt,
			UpdatedAt = entity.UpdatedAt
		};
	}
}
