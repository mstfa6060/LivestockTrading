using LivestockTrading.Domain.Entities;

namespace LivestockTrading.Application.RequestHandlers.Deals.Queries.All;

public class Mapper
{
	public List<ResponseModel> MapToResponse(List<Deal> deals)
	{
		return deals.Select(d => new ResponseModel
		{
			Id = d.Id,
			DealNumber = d.DealNumber,
			ProductId = d.ProductId,
			SellerId = d.SellerId,
			BuyerId = d.BuyerId,
			AgreedPrice = d.AgreedPrice,
			Currency = d.Currency,
			Quantity = d.Quantity,
			Status = (int)d.Status,
			DealDate = d.DealDate,
			DeliveryMethod = (int)d.DeliveryMethod,
			IsCompleted = d.IsCompleted,
			IsCancelled = d.IsCancelled,
			CreatedAt = d.CreatedAt
		}).ToList();
	}
}
