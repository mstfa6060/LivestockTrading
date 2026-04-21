using LivestockTrading.Domain.Entities;

namespace LivestockTrading.Application.RequestHandlers.FavoriteProducts.Commands.Create;

public class Mapper
{
	public FavoriteProduct MapToEntity(RequestModel request)
	{
		return new FavoriteProduct
		{
			Id = Guid.NewGuid(),
			UserId = request.UserId,
			ProductId = request.ProductId,
			AddedAt = DateTime.UtcNow,
			CreatedAt = DateTime.UtcNow
		};
	}

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
