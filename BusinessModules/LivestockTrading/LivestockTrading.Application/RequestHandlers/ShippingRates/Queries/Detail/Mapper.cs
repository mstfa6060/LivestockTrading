using LivestockTrading.Domain.Entities;

namespace LivestockTrading.Application.RequestHandlers.ShippingRates.Queries.Detail;

public class Mapper
{
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
