using LivestockTrading.Domain.Entities;

namespace LivestockTrading.Application.RequestHandlers.Deals.Commands.Create;

public class Mapper
{
	public Deal MapToEntity(RequestModel request)
	{
		return new Deal
		{
			Id = Guid.NewGuid(),
			DealNumber = request.DealNumber,
			ProductId = request.ProductId,
			SellerId = request.SellerId,
			BuyerId = request.BuyerId,
			AgreedPrice = request.AgreedPrice,
			Currency = request.Currency,
			Quantity = request.Quantity,
			Status = (DealStatus)request.Status,
			DealDate = request.DealDate,
			DeliveryMethod = (DeliveryMethod)request.DeliveryMethod,
			DeliveryDate = request.DeliveryDate,
			BuyerNotes = request.BuyerNotes,
			SellerNotes = request.SellerNotes,
			ContractDocuments = request.ContractDocuments,
			CreatedAt = DateTime.UtcNow
		};
	}

	public ResponseModel MapToResponse(Deal entity)
	{
		return new ResponseModel
		{
			Id = entity.Id,
			DealNumber = entity.DealNumber,
			ProductId = entity.ProductId,
			SellerId = entity.SellerId,
			BuyerId = entity.BuyerId,
			AgreedPrice = entity.AgreedPrice,
			Currency = entity.Currency,
			Quantity = entity.Quantity,
			Status = (int)entity.Status,
			DealDate = entity.DealDate,
			DeliveryMethod = (int)entity.DeliveryMethod,
			DeliveryDate = entity.DeliveryDate,
			BuyerNotes = entity.BuyerNotes,
			SellerNotes = entity.SellerNotes,
			ContractDocuments = entity.ContractDocuments,
			CreatedAt = entity.CreatedAt
		};
	}
}
