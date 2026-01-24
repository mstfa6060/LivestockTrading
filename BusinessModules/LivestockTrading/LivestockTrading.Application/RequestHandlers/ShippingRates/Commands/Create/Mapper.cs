using LivestockTrading.Domain.Entities;

namespace LivestockTrading.Application.RequestHandlers.ShippingRates.Commands.Create;

public class Mapper
{
	public ShippingRate MapToEntity(RequestModel request)
	{
		return new ShippingRate
		{
			Id = Guid.NewGuid(),
			ShippingZoneId = request.ShippingZoneId,
			ShippingCarrierId = request.ShippingCarrierId,
			MinWeight = request.MinWeight,
			MaxWeight = request.MaxWeight,
			MinOrderAmount = request.MinOrderAmount,
			ShippingCost = request.ShippingCost,
			Currency = request.Currency,
			EstimatedDeliveryDays = request.EstimatedDeliveryDays,
			IsFreeShipping = request.IsFreeShipping,
			IsActive = request.IsActive,
			CreatedAt = DateTime.UtcNow
		};
	}

	public ResponseModel MapToResponse(ShippingRate entity)
	{
		return new ResponseModel
		{
			Id = entity.Id,
			ShippingZoneId = entity.ShippingZoneId,
			ShippingCarrierId = entity.ShippingCarrierId,
			MinWeight = entity.MinWeight,
			MaxWeight = entity.MaxWeight,
			MinOrderAmount = entity.MinOrderAmount,
			ShippingCost = entity.ShippingCost,
			Currency = entity.Currency,
			EstimatedDeliveryDays = entity.EstimatedDeliveryDays,
			IsFreeShipping = entity.IsFreeShipping,
			IsActive = entity.IsActive,
			CreatedAt = entity.CreatedAt
		};
	}
}
