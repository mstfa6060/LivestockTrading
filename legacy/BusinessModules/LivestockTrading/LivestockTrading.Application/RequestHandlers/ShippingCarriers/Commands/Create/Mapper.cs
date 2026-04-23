using LivestockTrading.Domain.Entities;

namespace LivestockTrading.Application.RequestHandlers.ShippingCarriers.Commands.Create;

public class Mapper
{
	public ShippingCarrier MapToEntity(RequestModel request)
	{
		return new ShippingCarrier
		{
			Id = Guid.NewGuid(),
			Name = request.Name,
			Code = request.Code,
			Website = request.Website,
			TrackingUrlTemplate = request.TrackingUrlTemplate,
			IsActive = request.IsActive,
			SupportedCountries = request.SupportedCountries,
			CreatedAt = DateTime.UtcNow
		};
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
			CreatedAt = entity.CreatedAt
		};
	}
}
