using LivestockTrading.Domain.Entities;

namespace LivestockTrading.Application.RequestHandlers.Deals.Queries.Detail;

public class Mapper
{
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
			TransportRequestCreated = entity.TransportRequestCreated,
			TransportRequestId = entity.TransportRequestId,
			IsCompleted = entity.IsCompleted,
			CompletedAt = entity.CompletedAt,
			IsCancelled = entity.IsCancelled,
			CancelledAt = entity.CancelledAt,
			CancellationReason = entity.CancellationReason,
			CreatedAt = entity.CreatedAt,
			UpdatedAt = entity.UpdatedAt
		};
	}
}
