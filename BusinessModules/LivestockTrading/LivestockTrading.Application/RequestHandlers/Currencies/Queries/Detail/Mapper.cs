using LivestockTrading.Domain.Entities;

namespace LivestockTrading.Application.RequestHandlers.Currencies.Queries.Detail;

public class Mapper
{
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
			CreatedAt = entity.CreatedAt,
			UpdatedAt = entity.UpdatedAt
		};
	}
}
