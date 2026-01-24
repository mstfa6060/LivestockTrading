using LivestockTrading.Domain.Entities;

namespace LivestockTrading.Application.RequestHandlers.TransportOffers.Queries.Pick;

public class Mapper
{
	public List<ResponseModel> MapToResponse(List<TransportOffer> items)
	{
		return items.Select(e => new ResponseModel
		{
			Id = e.Id,
			TransporterId = e.TransporterId,
			OfferedPrice = e.OfferedPrice
		}).ToList();
	}
}
