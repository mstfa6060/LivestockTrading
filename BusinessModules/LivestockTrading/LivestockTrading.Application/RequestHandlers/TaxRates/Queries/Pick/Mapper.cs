using LivestockTrading.Domain.Entities;

namespace LivestockTrading.Application.RequestHandlers.TaxRates.Queries.Pick;

public class Mapper
{
	public List<ResponseModel> MapToResponse(List<TaxRate> items)
	{
		return items.Select(e => new ResponseModel
		{
			Id = e.Id,
			TaxName = e.TaxName,
			CountryCode = e.CountryCode,
			Rate = e.Rate
		}).ToList();
	}
}
