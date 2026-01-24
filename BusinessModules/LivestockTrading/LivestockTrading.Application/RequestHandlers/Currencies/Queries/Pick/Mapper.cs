using LivestockTrading.Domain.Entities;

namespace LivestockTrading.Application.RequestHandlers.Currencies.Queries.Pick;

public class Mapper
{
	public List<ResponseModel> MapToResponse(List<Currency> currencies)
	{
		return currencies.Select(c => new ResponseModel
		{
			Id = c.Id,
			Code = c.Code,
			Name = c.Name,
			Symbol = c.Symbol
		}).ToList();
	}
}
