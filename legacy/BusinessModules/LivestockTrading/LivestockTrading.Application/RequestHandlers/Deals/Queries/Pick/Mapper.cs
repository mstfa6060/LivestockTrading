using LivestockTrading.Domain.Entities;

namespace LivestockTrading.Application.RequestHandlers.Deals.Queries.Pick;

public class Mapper
{
	public List<ResponseModel> MapToResponse(List<Deal> deals)
	{
		return deals.Select(d => new ResponseModel
		{
			Id = d.Id,
			DealNumber = d.DealNumber,
			AgreedPrice = d.AgreedPrice
		}).ToList();
	}
}
