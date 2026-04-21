using LivestockTrading.Domain.Entities;

namespace LivestockTrading.Application.RequestHandlers.FavoriteProducts.Queries.Detail;

public class Mapper
{
	public ResponseModel MapToResponse(FavoriteProduct entity)
	{
		return new ResponseModel
		{
			Id = entity.Id,
			UserId = entity.UserId,
			ProductId = entity.ProductId,
			AddedAt = entity.AddedAt,
			CreatedAt = entity.CreatedAt
		};
	}
}
