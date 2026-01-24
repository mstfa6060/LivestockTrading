using LivestockTrading.Domain.Entities;

namespace LivestockTrading.Application.RequestHandlers.TransportRequests.Queries.Pick;

public class Mapper
{
	public List<ResponseModel> MapToResponse(List<TransportRequest> items)
	{
		return items.Select(e => new ResponseModel
		{
			Id = e.Id,
			ProductId = e.ProductId,
			Status = (int)e.Status
		}).ToList();
	}
}
