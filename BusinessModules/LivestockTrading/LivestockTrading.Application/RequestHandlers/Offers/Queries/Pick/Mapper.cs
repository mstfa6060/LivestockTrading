using LivestockTrading.Domain.Entities;

namespace LivestockTrading.Application.RequestHandlers.Offers.Queries.Pick;

public class Mapper
{
	public List<ResponseModel> MapToResponse(List<Offer> offers)
	{
		return offers.Select(o => new ResponseModel
		{
			Id = o.Id,
			ProductId = o.ProductId,
			OfferedPrice = o.OfferedPrice
		}).ToList();
	}
}
