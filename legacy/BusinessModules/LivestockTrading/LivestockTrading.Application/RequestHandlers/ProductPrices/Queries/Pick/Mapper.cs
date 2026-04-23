using LivestockTrading.Domain.Entities;

namespace LivestockTrading.Application.RequestHandlers.ProductPrices.Queries.Pick;

public class Mapper
{
	public List<ResponseModel> MapToResponse(List<ProductPrice> items)
	{
		return items.Select(e => new ResponseModel
		{
			Id = e.Id,
			CurrencyCode = e.CurrencyCode,
			Price = e.Price
		}).ToList();
	}
}
