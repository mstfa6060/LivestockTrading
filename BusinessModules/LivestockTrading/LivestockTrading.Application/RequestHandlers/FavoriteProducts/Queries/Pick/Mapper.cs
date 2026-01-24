using LivestockTrading.Domain.Entities;

namespace LivestockTrading.Application.RequestHandlers.FavoriteProducts.Queries.Pick;

public class Mapper
{
	public List<ResponseModel> MapToResponse(List<FavoriteProduct> items)
	{
		return items.Select(entity => new ResponseModel
		{
			Id = entity.Id,
			ProductId = entity.ProductId,
			AddedAt = entity.AddedAt
		}).ToList();
	}
}
