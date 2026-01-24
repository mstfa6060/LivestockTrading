using LivestockTrading.Domain.Entities;

namespace LivestockTrading.Application.RequestHandlers.ProductViewHistories.Queries.All;

public class Mapper
{
	public List<ResponseModel> MapToResponse(List<ProductViewHistory> items)
	{
		return items.Select(e => new ResponseModel
		{
			Id = e.Id,
			UserId = e.UserId,
			ProductId = e.ProductId,
			ViewedAt = e.ViewedAt,
			ViewSource = e.ViewSource,
			CreatedAt = e.CreatedAt
		}).ToList();
	}
}
