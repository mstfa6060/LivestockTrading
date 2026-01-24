using LivestockTrading.Domain.Entities;

namespace LivestockTrading.Application.RequestHandlers.TaxRates.Queries.All;

public class Mapper
{
	public List<ResponseModel> MapToResponse(List<TaxRate> items)
	{
		return items.Select(e => new ResponseModel
		{
			Id = e.Id,
			CountryCode = e.CountryCode,
			StateCode = e.StateCode,
			TaxName = e.TaxName,
			Rate = e.Rate,
			Type = (int)e.Type,
			CategoryId = e.CategoryId,
			IsActive = e.IsActive,
			ValidFrom = e.ValidFrom,
			ValidUntil = e.ValidUntil,
			CreatedAt = e.CreatedAt
		}).ToList();
	}
}
