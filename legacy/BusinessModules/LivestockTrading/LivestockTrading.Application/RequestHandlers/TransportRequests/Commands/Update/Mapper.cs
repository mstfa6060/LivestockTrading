using LivestockTrading.Domain.Entities;

namespace LivestockTrading.Application.RequestHandlers.TransportRequests.Commands.Update;

public class Mapper
{
	public void MapToEntity(RequestModel request, TransportRequest entity)
	{
		entity.ProductId = request.ProductId;
		entity.SellerId = request.SellerId;
		entity.BuyerId = request.BuyerId;
		entity.AgreedPrice = request.AgreedPrice;
		entity.Currency = request.Currency;
		entity.PickupLocationId = request.PickupLocationId;
		entity.DeliveryLocationId = request.DeliveryLocationId;
		entity.EstimatedDistanceKm = request.EstimatedDistanceKm;
		entity.WeightKg = request.WeightKg;
		entity.VolumeCubicMeters = request.VolumeCubicMeters;
		entity.SpecialInstructions = request.SpecialInstructions;
		entity.PreferredPickupDate = request.PreferredPickupDate;
		entity.PreferredDeliveryDate = request.PreferredDeliveryDate;
		entity.IsUrgent = request.IsUrgent;
		entity.TransportType = (TransportType)request.TransportType;
		entity.Status = (TransportRequestStatus)request.Status;
		entity.Notes = request.Notes;
		entity.UpdatedAt = DateTime.UtcNow;
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
			UpdatedAt = entity.UpdatedAt
		};
	}
}
