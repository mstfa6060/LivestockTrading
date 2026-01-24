using LivestockTrading.Domain.Entities;

namespace LivestockTrading.Application.RequestHandlers.TransportOffers.Queries.All;

public class Mapper
{
	public List<ResponseModel> MapToResponse(List<TransportOffer> items)
	{
		return items.Select(e => new ResponseModel
		{
			Id = e.Id,
			TransportRequestId = e.TransportRequestId,
			TransporterId = e.TransporterId,
			OfferedPrice = e.OfferedPrice,
			Currency = e.Currency,
			Status = (int)e.Status,
			EstimatedDurationDays = e.EstimatedDurationDays,
			InsuranceIncluded = e.InsuranceIncluded,
			OfferDate = e.OfferDate,
			CreatedAt = e.CreatedAt
		}).ToList();
	}
}
