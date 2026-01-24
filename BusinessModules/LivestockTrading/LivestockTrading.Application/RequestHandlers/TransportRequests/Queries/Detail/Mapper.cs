using LivestockTrading.Domain.Entities;

namespace LivestockTrading.Application.RequestHandlers.TransportRequests.Queries.Detail;

public class Mapper
{
	public ResponseModel MapToResponse(TransportRequest entity)
	{
		return new ResponseModel
		{
			Id = entity.Id,
			ProductId = entity.ProductId,
			SellerId = entity.SellerId,
			BuyerId = entity.BuyerId,
			AgreedPrice = entity.AgreedPrice,
			Currency = entity.Currency,
			PickupLocationId = entity.PickupLocationId,
			DeliveryLocationId = entity.DeliveryLocationId,
			EstimatedDistanceKm = entity.EstimatedDistanceKm,
			WeightKg = entity.WeightKg,
			VolumeCubicMeters = entity.VolumeCubicMeters,
			SpecialInstructions = entity.SpecialInstructions,
			PreferredPickupDate = entity.PreferredPickupDate,
			PreferredDeliveryDate = entity.PreferredDeliveryDate,
			IsUrgent = entity.IsUrgent,
			TransportType = (int)entity.TransportType,
			Status = (int)entity.Status,
			IsInPool = entity.IsInPool,
			AddedToPoolAt = entity.AddedToPoolAt,
			AssignedTransporterId = entity.AssignedTransporterId,
			AssignedAt = entity.AssignedAt,
			PickedUpAt = entity.PickedUpAt,
			DeliveredAt = entity.DeliveredAt,
			CancelledAt = entity.CancelledAt,
			CancellationReason = entity.CancellationReason,
			Notes = entity.Notes,
			CreatedAt = entity.CreatedAt,
			UpdatedAt = entity.UpdatedAt
		};
	}
}
