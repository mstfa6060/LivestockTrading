using LivestockTrading.Domain.Entities;

namespace LivestockTrading.Application.RequestHandlers.TransportOffers.Commands.Update;

public class Mapper
{
	public void MapToEntity(RequestModel request, TransportOffer entity)
	{
		entity.TransportRequestId = request.TransportRequestId;
		entity.TransporterId = request.TransporterId;
		entity.OfferedPrice = request.OfferedPrice;
		entity.Currency = request.Currency;
		entity.EstimatedPickupDate = request.EstimatedPickupDate;
		entity.EstimatedDeliveryDate = request.EstimatedDeliveryDate;
		entity.EstimatedDurationDays = request.EstimatedDurationDays;
		entity.VehicleType = request.VehicleType;
		entity.InsuranceIncluded = request.InsuranceIncluded;
		entity.InsuranceAmount = request.InsuranceAmount;
		entity.AdditionalServices = request.AdditionalServices;
		entity.Message = request.Message;
		entity.Status = (TransportOfferStatus)request.Status;
		entity.ExpiryDate = request.ExpiryDate;
		entity.UpdatedAt = DateTime.UtcNow;
	}

	public ResponseModel MapToResponse(TransportOffer entity)
	{
		return new ResponseModel
		{
			Id = entity.Id,
			TransportRequestId = entity.TransportRequestId,
			TransporterId = entity.TransporterId,
			OfferedPrice = entity.OfferedPrice,
			Currency = entity.Currency,
			EstimatedPickupDate = entity.EstimatedPickupDate,
			EstimatedDeliveryDate = entity.EstimatedDeliveryDate,
			EstimatedDurationDays = entity.EstimatedDurationDays,
			VehicleType = entity.VehicleType,
			InsuranceIncluded = entity.InsuranceIncluded,
			InsuranceAmount = entity.InsuranceAmount,
			AdditionalServices = entity.AdditionalServices,
			Message = entity.Message,
			Status = (int)entity.Status,
			OfferDate = entity.OfferDate,
			ExpiryDate = entity.ExpiryDate,
			CreatedAt = entity.CreatedAt,
			UpdatedAt = entity.UpdatedAt
		};
	}
}
