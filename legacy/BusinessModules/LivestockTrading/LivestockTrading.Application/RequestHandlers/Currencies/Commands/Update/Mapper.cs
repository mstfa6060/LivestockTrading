using LivestockTrading.Domain.Entities;

namespace LivestockTrading.Application.RequestHandlers.Currencies.Commands.Update;

public class Mapper
{
	public void MapToEntity(RequestModel request, Currency entity)
	{
		entity.Code = request.Code;
		entity.Symbol = request.Symbol;
		entity.Name = request.Name;
		entity.ExchangeRateToUSD = request.ExchangeRateToUSD;
		entity.IsActive = request.IsActive;
		entity.LastUpdated = DateTime.UtcNow;
		entity.UpdatedAt = DateTime.UtcNow;
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
			UpdatedAt = entity.UpdatedAt
		};
	}
}
