using LivestockTrading.Domain.Entities;

namespace LivestockTrading.Application.RequestHandlers.ProductPrices.Commands.Update;

public class Mapper
{
	public void MapToEntity(RequestModel request, ProductPrice entity)
	{
		entity.ProductId = request.ProductId;
		entity.CurrencyCode = request.CurrencyCode;
		entity.Price = request.Price;
		entity.DiscountedPrice = request.DiscountedPrice;
		entity.CountryCodes = request.CountryCodes;
		entity.IsActive = request.IsActive;
		entity.ValidFrom = request.ValidFrom;
		entity.ValidUntil = request.ValidUntil;
		entity.IsAutomaticConversion = request.IsAutomaticConversion;
		entity.UpdatedAt = DateTime.UtcNow;
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
			UpdatedAt = entity.UpdatedAt
		};
	}
}
