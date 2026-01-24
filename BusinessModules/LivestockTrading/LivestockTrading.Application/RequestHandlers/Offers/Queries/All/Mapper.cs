using LivestockTrading.Domain.Entities;

namespace LivestockTrading.Application.RequestHandlers.Offers.Queries.All;

public class Mapper
{
	public List<ResponseModel> MapToResponse(List<Offer> offers)
	{
		return offers.Select(o => new ResponseModel
		{
			Id = o.Id,
			ProductId = o.ProductId,
			BuyerUserId = o.BuyerUserId,
			SellerUserId = o.SellerUserId,
			OfferedPrice = o.OfferedPrice,
			Currency = o.Currency,
			Quantity = o.Quantity,
			Status = (int)o.Status,
			OfferDate = o.OfferDate,
			ExpiryDate = o.ExpiryDate,
			CreatedAt = o.CreatedAt
		}).ToList();
	}
}
