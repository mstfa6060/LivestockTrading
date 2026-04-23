using LivestockTrading.Domain.Entities;

namespace LivestockTrading.Application.RequestHandlers.TransportRequests.Commands.Create;

public class Mapper
{
	public TransportRequest MapToEntity(RequestModel request)
	{
		return new TransportRequest
		{
			Id = Guid.NewGuid(),
			ProductId = request.ProductId,
			SellerId = request.SellerId,
			BuyerId = request.BuyerId,
			AgreedPrice = request.AgreedPrice,
			Currency = request.Currency,
			PickupLocationId = request.PickupLocationId,
			DeliveryLocationId = request.DeliveryLocationId,
			EstimatedDistanceKm = request.EstimatedDistanceKm,
			WeightKg = request.WeightKg,
			VolumeCubicMeters = request.VolumeCubicMeters,
			SpecialInstructions = request.SpecialInstructions,
			PreferredPickupDate = request.PreferredPickupDate,
			PreferredDeliveryDate = request.PreferredDeliveryDate,
			IsUrgent = request.IsUrgent,
			TransportType = (TransportType)request.TransportType,
			Status = (TransportRequestStatus)request.Status,
			Notes = request.Notes,
			CreatedAt = DateTime.UtcNow
		};
	}

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
			Notes = entity.Notes,
			CreatedAt = entity.CreatedAt
		};
	}
}
