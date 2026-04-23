using LivestockTrading.Domain.Entities;

namespace LivestockTrading.Application.RequestHandlers.ShippingRates.Queries.Pick;

public class Mapper
{
	public List<ResponseModel> MapToResponse(List<ShippingRate> items)
	{
		return items.Select(e => new ResponseModel
		{
			Id = e.Id,
			ShippingCost = e.ShippingCost,
			Currency = e.Currency
		}).ToList();
	}
}
