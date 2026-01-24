using LivestockTrading.Domain.Entities;

namespace LivestockTrading.Application.RequestHandlers.ShippingRates.Commands.Update;

public class Mapper
{
	public void MapToEntity(RequestModel request, ShippingRate entity)
	{
		entity.ShippingZoneId = request.ShippingZoneId;
		entity.ShippingCarrierId = request.ShippingCarrierId;
		entity.MinWeight = request.MinWeight;
		entity.MaxWeight = request.MaxWeight;
		entity.MinOrderAmount = request.MinOrderAmount;
		entity.ShippingCost = request.ShippingCost;
		entity.Currency = request.Currency;
		entity.EstimatedDeliveryDays = request.EstimatedDeliveryDays;
		entity.IsFreeShipping = request.IsFreeShipping;
		entity.IsActive = request.IsActive;
		entity.UpdatedAt = DateTime.UtcNow;
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
			CreatedAt = entity.CreatedAt,
			UpdatedAt = entity.UpdatedAt
		};
	}
}
