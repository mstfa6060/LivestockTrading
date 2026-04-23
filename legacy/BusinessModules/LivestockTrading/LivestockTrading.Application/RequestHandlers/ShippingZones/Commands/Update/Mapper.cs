using LivestockTrading.Domain.Entities;

namespace LivestockTrading.Application.RequestHandlers.ShippingZones.Commands.Update;

public class Mapper
{
	public void MapToEntity(RequestModel request, ShippingZone entity)
	{
		entity.SellerId = request.SellerId;
		entity.Name = request.Name;
		entity.CountryCodes = request.CountryCodes;
		entity.IsActive = request.IsActive;
		entity.UpdatedAt = DateTime.UtcNow;
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
			CreatedAt = entity.CreatedAt,
			UpdatedAt = entity.UpdatedAt
		};
	}
}
