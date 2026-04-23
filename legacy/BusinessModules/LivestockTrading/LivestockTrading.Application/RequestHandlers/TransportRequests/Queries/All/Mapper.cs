using LivestockTrading.Domain.Entities;

namespace LivestockTrading.Application.RequestHandlers.TransportRequests.Queries.All;

public class Mapper
{
	public List<ResponseModel> MapToResponse(List<TransportRequest> items)
	{
		return items.Select(e => new ResponseModel
		{
			Id = e.Id,
			ProductId = e.ProductId,
			SellerId = e.SellerId,
			BuyerId = e.BuyerId,
			PickupLocationId = e.PickupLocationId,
			DeliveryLocationId = e.DeliveryLocationId,
			TransportType = (int)e.TransportType,
			Status = (int)e.Status,
			IsUrgent = e.IsUrgent,
			IsInPool = e.IsInPool,
			AssignedTransporterId = e.AssignedTransporterId,
			CreatedAt = e.CreatedAt
		}).ToList();
	}
}
