using LivestockTrading.Domain.Entities;

namespace LivestockTrading.Application.RequestHandlers.ShippingZones.Queries.All;

public class Mapper
{
	public List<ResponseModel> MapToResponse(List<ShippingZone> items)
	{
		return items.Select(e => new ResponseModel
		{
			Id = e.Id,
			SellerId = e.SellerId,
			Name = e.Name,
			CountryCodes = e.CountryCodes,
			IsActive = e.IsActive,
			CreatedAt = e.CreatedAt,
			UpdatedAt = e.UpdatedAt
		}).ToList();
	}
}
