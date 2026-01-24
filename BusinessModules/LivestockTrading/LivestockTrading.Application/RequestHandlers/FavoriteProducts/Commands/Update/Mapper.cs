using LivestockTrading.Domain.Entities;

namespace LivestockTrading.Application.RequestHandlers.FavoriteProducts.Commands.Update;

public class Mapper
{
	public void MapToEntity(RequestModel request, FavoriteProduct entity)
	{
		entity.UserId = request.UserId;
		entity.ProductId = request.ProductId;
		entity.UpdatedAt = DateTime.UtcNow;
	}

	public ResponseModel MapToResponse(FavoriteProduct entity)
	{
		return new ResponseModel
		{
			Id = entity.Id,
			UserId = entity.UserId,
			ProductId = entity.ProductId,
			AddedAt = entity.AddedAt,
			UpdatedAt = entity.UpdatedAt
		};
	}
}
