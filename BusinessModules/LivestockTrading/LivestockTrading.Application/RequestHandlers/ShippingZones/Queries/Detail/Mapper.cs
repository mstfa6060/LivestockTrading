using LivestockTrading.Domain.Entities;

namespace LivestockTrading.Application.RequestHandlers.ShippingZones.Queries.Detail;

public class Mapper
{
	public ResponseModel MapToResponse(ShippingZone entity)
	{
		return new ResponseModel
		{
			Id = entity.Id,
			SellerId = entity.SellerId,
			Name = entity.Name,
			CountryCodes = entity.CountryCodes,
			IsActive = entity.IsActive,
			CreatedAt = entity.CreatedAt,
			UpdatedAt = entity.UpdatedAt
		};
	}
}
