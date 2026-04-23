using LivestockTrading.Domain.Entities;

namespace LivestockTrading.Application.RequestHandlers.ShippingCarriers.Commands.Update;

public class Mapper
{
	public void MapToEntity(RequestModel request, ShippingCarrier entity)
	{
		entity.Name = request.Name;
		entity.Code = request.Code;
		entity.Website = request.Website;
		entity.TrackingUrlTemplate = request.TrackingUrlTemplate;
		entity.IsActive = request.IsActive;
		entity.SupportedCountries = request.SupportedCountries;
		entity.UpdatedAt = DateTime.UtcNow;
	}

	public ResponseModel MapToResponse(ShippingCarrier entity)
	{
		return new ResponseModel
		{
			Id = entity.Id,
			Name = entity.Name,
			Code = entity.Code,
			Website = entity.Website,
			TrackingUrlTemplate = entity.TrackingUrlTemplate,
			IsActive = entity.IsActive,
			SupportedCountries = entity.SupportedCountries,
			UpdatedAt = entity.UpdatedAt
		};
	}
}
