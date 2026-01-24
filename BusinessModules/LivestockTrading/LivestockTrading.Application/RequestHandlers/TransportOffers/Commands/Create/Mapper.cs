using LivestockTrading.Domain.Entities;

namespace LivestockTrading.Application.RequestHandlers.TransportOffers.Commands.Create;

public class Mapper
{
	public TransportOffer MapToEntity(RequestModel request)
	{
		return new TransportOffer
		{
			Id = Guid.NewGuid(),
			TransportRequestId = request.TransportRequestId,
			TransporterId = request.TransporterId,
			OfferedPrice = request.OfferedPrice,
			Currency = request.Currency,
			EstimatedPickupDate = request.EstimatedPickupDate,
			EstimatedDeliveryDate = request.EstimatedDeliveryDate,
			EstimatedDurationDays = request.EstimatedDurationDays,
			VehicleType = request.VehicleType,
			InsuranceIncluded = request.InsuranceIncluded,
			InsuranceAmount = request.InsuranceAmount,
			AdditionalServices = request.AdditionalServices,
			Message = request.Message,
			Status = (TransportOfferStatus)request.Status,
			OfferDate = DateTime.UtcNow,
			ExpiryDate = request.ExpiryDate,
			CreatedAt = DateTime.UtcNow
		};
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
			CreatedAt = entity.CreatedAt
		};
	}
}
