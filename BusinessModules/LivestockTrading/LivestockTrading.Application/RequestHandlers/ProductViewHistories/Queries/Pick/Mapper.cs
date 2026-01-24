using LivestockTrading.Domain.Entities;

namespace LivestockTrading.Application.RequestHandlers.ProductViewHistories.Queries.Pick;

public class Mapper
{
	public List<ResponseModel> MapToResponse(List<ProductViewHistory> items)
	{
		return items.Select(e => new ResponseModel
		{
			Id = e.Id,
			ProductId = e.ProductId,
			ViewedAt = e.ViewedAt
		}).ToList();
	}
}
