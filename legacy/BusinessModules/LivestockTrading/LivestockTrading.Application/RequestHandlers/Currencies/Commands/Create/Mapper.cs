using LivestockTrading.Domain.Entities;

namespace LivestockTrading.Application.RequestHandlers.Currencies.Commands.Create;

public class Mapper
{
	public Currency MapToEntity(RequestModel request)
	{
		return new Currency
		{
			Id = Guid.NewGuid(),
			Code = request.Code,
			Symbol = request.Symbol,
			Name = request.Name,
			ExchangeRateToUSD = request.ExchangeRateToUSD,
			LastUpdated = DateTime.UtcNow,
			IsActive = request.IsActive,
			CreatedAt = DateTime.UtcNow
		};
	}

	public ResponseModel MapToResponse(Currency entity)
	{
		return new ResponseModel
		{
			Id = entity.Id,
			Code = entity.Code,
			Symbol = entity.Symbol,
			Name = entity.Name,
			ExchangeRateToUSD = entity.ExchangeRateToUSD,
			LastUpdated = entity.LastUpdated,
			IsActive = entity.IsActive,
			CreatedAt = entity.CreatedAt
		};
	}
}
