using LivestockTrading.Domain.Entities;

namespace LivestockTrading.Application.RequestHandlers.Offers.Queries.Detail;

public class Mapper
{
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
			Status = (int)entity.Status,
			OfferDate = entity.OfferDate,
			ExpiryDate = entity.ExpiryDate,
			CounterOfferToId = entity.CounterOfferToId,
			RespondedAt = entity.RespondedAt,
			ResponseMessage = entity.ResponseMessage,
			CreatedAt = entity.CreatedAt,
			UpdatedAt = entity.UpdatedAt
		};
	}
}
