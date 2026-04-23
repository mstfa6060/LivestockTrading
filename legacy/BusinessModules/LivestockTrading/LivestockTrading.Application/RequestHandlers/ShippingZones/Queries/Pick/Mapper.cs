using LivestockTrading.Domain.Entities;

namespace LivestockTrading.Application.RequestHandlers.ShippingZones.Queries.Pick;

public class Mapper
{
	public List<ResponseModel> MapToResponse(List<ShippingZone> items)
	{
		return items.Select(e => new ResponseModel
		{
			Id = e.Id,
			Name = e.Name
		}).ToList();
	}
}
