using LivestockTrading.Domain.Entities;

namespace LivestockTrading.Application.RequestHandlers.ShippingCarriers.Queries.Pick;

public class Mapper
{
	public List<ResponseModel> MapToResponse(List<ShippingCarrier> items)
	{
		return items.Select(s => new ResponseModel
		{
			Id = s.Id,
			Name = s.Name,
			Code = s.Code
		}).ToList();
	}
}
