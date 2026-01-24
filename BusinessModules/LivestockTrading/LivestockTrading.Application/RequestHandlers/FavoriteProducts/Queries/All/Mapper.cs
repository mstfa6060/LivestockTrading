using LivestockTrading.Domain.Entities;

namespace LivestockTrading.Application.RequestHandlers.FavoriteProducts.Queries.All;

public class Mapper
{
	public List<ResponseModel> MapToResponse(List<FavoriteProduct> items)
	{
		return items.Select(entity => new ResponseModel
		{
			Id = entity.Id,
			UserId = entity.UserId,
			ProductId = entity.ProductId,
			AddedAt = entity.AddedAt,
			CreatedAt = entity.CreatedAt
		}).ToList();
	}
}
