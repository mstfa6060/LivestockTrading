using LivestockTrading.Domain.Entities;

namespace LivestockTrading.Application.RequestHandlers.ShippingCarriers.Queries.Detail;

public class Mapper
{
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
			CreatedAt = entity.CreatedAt,
			UpdatedAt = entity.UpdatedAt
		};
	}
}
