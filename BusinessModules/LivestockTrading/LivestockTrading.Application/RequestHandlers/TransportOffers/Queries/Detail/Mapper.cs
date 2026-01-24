using LivestockTrading.Domain.Entities;

namespace LivestockTrading.Application.RequestHandlers.TransportOffers.Queries.Detail;

public class Mapper
{
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
			RespondedAt = entity.RespondedAt,
			ResponseMessage = entity.ResponseMessage,
			CreatedAt = entity.CreatedAt,
			UpdatedAt = entity.UpdatedAt
		};
	}
}
