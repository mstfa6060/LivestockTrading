using LivestockTrading.Domain.Entities;

namespace LivestockTrading.Application.RequestHandlers.ProductPrices.Commands.Create;

public class Mapper
{
	public ProductPrice MapToEntity(RequestModel request)
	{
		return new ProductPrice
		{
			Id = Guid.NewGuid(),
			ProductId = request.ProductId,
			CurrencyCode = request.CurrencyCode,
			Price = request.Price,
			DiscountedPrice = request.DiscountedPrice,
			CountryCodes = request.CountryCodes,
			IsActive = request.IsActive,
			ValidFrom = request.ValidFrom,
			ValidUntil = request.ValidUntil,
			IsAutomaticConversion = request.IsAutomaticConversion,
			CreatedAt = DateTime.UtcNow
		};
	}

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
			CreatedAt = entity.CreatedAt
		};
	}
}
