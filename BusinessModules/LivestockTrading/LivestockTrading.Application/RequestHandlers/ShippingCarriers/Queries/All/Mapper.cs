using LivestockTrading.Domain.Entities;

namespace LivestockTrading.Application.RequestHandlers.ShippingCarriers.Queries.All;

public class Mapper
{
	public List<ResponseModel> MapToResponse(List<ShippingCarrier> items)
	{
		return items.Select(s => new ResponseModel
		{
			Id = s.Id,
			Name = s.Name,
			Code = s.Code,
			Website = s.Website,
			TrackingUrlTemplate = s.TrackingUrlTemplate,
			IsActive = s.IsActive,
			SupportedCountries = s.SupportedCountries,
			CreatedAt = s.CreatedAt
		}).ToList();
	}
}
