using LivestockTrading.Domain.Entities;

namespace LivestockTrading.Application.RequestHandlers.Offers.Commands.Create;

public class Mapper
{
	public Offer MapToEntity(RequestModel request)
	{
		return new Offer
		{
			Id = Guid.NewGuid(),
			ProductId = request.ProductId,
			BuyerUserId = request.BuyerUserId,
			SellerUserId = request.SellerUserId,
			OfferedPrice = request.OfferedPrice,
			Currency = request.Currency,
			Quantity = request.Quantity,
			Message = request.Message,
			Status = request.Status,
			OfferDate = DateTime.UtcNow,
			ExpiryDate = request.ExpiryDate,
			CounterOfferToId = request.CounterOfferToId,
			CreatedAt = DateTime.UtcNow
		};
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
			CreatedAt = entity.CreatedAt
		};
	}
}
