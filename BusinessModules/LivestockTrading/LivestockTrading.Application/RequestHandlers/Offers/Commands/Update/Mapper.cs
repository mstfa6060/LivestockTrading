using LivestockTrading.Domain.Entities;

namespace LivestockTrading.Application.RequestHandlers.Offers.Commands.Update;

public class Mapper
{
	public void MapToEntity(RequestModel request, Offer entity)
	{
		entity.ProductId = request.ProductId;
		entity.BuyerUserId = request.BuyerUserId;
		entity.SellerUserId = request.SellerUserId;
		entity.OfferedPrice = request.OfferedPrice;
		entity.Currency = request.Currency;
		entity.Quantity = request.Quantity;
		entity.Message = request.Message;
		entity.Status = request.Status;
		entity.ExpiryDate = request.ExpiryDate;
		entity.CounterOfferToId = request.CounterOfferToId;
		entity.RespondedAt = request.RespondedAt;
		entity.ResponseMessage = request.ResponseMessage;
		entity.UpdatedAt = DateTime.UtcNow;
	}

	public ResponseModel MapToResponse(Offer entity)
	{
		return new ResponseModel
		{
			Id = entity.Id,
			ProductId = entity.ProductId,
			BuyerUserId = entity.BuyerUserId,
			SellerUserId = entity.SellerUserId,
			OfferedPrice = entity.OfferedPrice,
			Currency = entity.Currency,
			Quantity = entity.Quantity,
			Message = entity.Message,
			Status = entity.Status,
			OfferDate = entity.OfferDate,
			ExpiryDate = entity.ExpiryDate,
			CounterOfferToId = entity.CounterOfferToId,
			RespondedAt = entity.RespondedAt,
			ResponseMessage = entity.ResponseMessage,
			UpdatedAt = entity.UpdatedAt
		};
	}
}
