using LivestockTrading.Domain.Entities;

namespace LivestockTrading.Application.RequestHandlers.ShippingZones.Commands.Create;

public class Mapper
{
	public ShippingZone MapToEntity(RequestModel request)
	{
		return new ShippingZone
		{
			Id = Guid.NewGuid(),
			SellerId = request.SellerId,
			Name = request.Name,
			CountryCodes = request.CountryCodes,
			IsActive = request.IsActive,
			CreatedAt = DateTime.UtcNow
		};
	}

	public ResponseModel MapToResponse(ShippingZone entity)
	{
		return new ResponseModel
		{
			Id = entity.Id,
			SellerId = entity.SellerId,
			Name = entity.Name,
			CountryCodes = entity.CountryCodes,
			IsActive = entity.IsActive,
			CreatedAt = entity.CreatedAt
		};
	}
}
