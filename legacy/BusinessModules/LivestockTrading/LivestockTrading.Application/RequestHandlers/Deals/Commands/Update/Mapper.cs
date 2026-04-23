using LivestockTrading.Domain.Entities;

namespace LivestockTrading.Application.RequestHandlers.Deals.Commands.Update;

public class Mapper
{
	public void MapToEntity(RequestModel request, Deal entity)
	{
		entity.DealNumber = request.DealNumber;
		entity.ProductId = request.ProductId;
		entity.SellerId = request.SellerId;
		entity.BuyerId = request.BuyerId;
		entity.AgreedPrice = request.AgreedPrice;
		entity.Currency = request.Currency;
		entity.Quantity = request.Quantity;
		entity.Status = (DealStatus)request.Status;
		entity.DealDate = request.DealDate;
		entity.DeliveryMethod = (DeliveryMethod)request.DeliveryMethod;
		entity.DeliveryDate = request.DeliveryDate;
		entity.BuyerNotes = request.BuyerNotes;
		entity.SellerNotes = request.SellerNotes;
		entity.ContractDocuments = request.ContractDocuments;
		entity.UpdatedAt = DateTime.UtcNow;
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
			UpdatedAt = entity.UpdatedAt
		};
	}
}
