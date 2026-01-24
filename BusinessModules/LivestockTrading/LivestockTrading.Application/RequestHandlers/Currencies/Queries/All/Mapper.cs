using LivestockTrading.Domain.Entities;

namespace LivestockTrading.Application.RequestHandlers.Currencies.Queries.All;

public class Mapper
{
	public List<ResponseModel> MapToResponse(List<Currency> currencies)
	{
		return currencies.Select(c => new ResponseModel
		{
			Id = c.Id,
			Code = c.Code,
			Symbol = c.Symbol,
			Name = c.Name,
			ExchangeRateToUSD = c.ExchangeRateToUSD,
			LastUpdated = c.LastUpdated,
			IsActive = c.IsActive,
			CreatedAt = c.CreatedAt
		}).ToList();
	}
}
