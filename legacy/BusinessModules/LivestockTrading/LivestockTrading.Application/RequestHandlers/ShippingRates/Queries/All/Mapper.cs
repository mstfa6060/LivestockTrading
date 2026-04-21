using LivestockTrading.Domain.Entities;

namespace LivestockTrading.Application.RequestHandlers.ShippingRates.Queries.All;

public class Mapper
{
	public List<ResponseModel> MapToResponse(List<ShippingRate> items)
	{
		return items.Select(e => new ResponseModel
		{
			Id = e.Id,
			ShippingZoneId = e.ShippingZoneId,
			ShippingCarrierId = e.ShippingCarrierId,
			MinWeight = e.MinWeight,
			MaxWeight = e.MaxWeight,
			MinOrderAmount = e.MinOrderAmount,
			ShippingCost = e.ShippingCost,
			Currency = e.Currency,
			EstimatedDeliveryDays = e.EstimatedDeliveryDays,
			IsFreeShipping = e.IsFreeShipping,
			IsActive = e.IsActive,
			CreatedAt = e.CreatedAt,
			UpdatedAt = e.UpdatedAt
		}).ToList();
	}
}
