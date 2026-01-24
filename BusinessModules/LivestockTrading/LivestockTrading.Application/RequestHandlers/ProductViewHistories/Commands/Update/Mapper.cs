using LivestockTrading.Domain.Entities;

namespace LivestockTrading.Application.RequestHandlers.ProductViewHistories.Commands.Update;

public class Mapper
{
	public void MapToEntity(RequestModel request, ProductViewHistory entity)
	{
		entity.UserId = request.UserId;
		entity.ProductId = request.ProductId;
		entity.ViewSource = request.ViewSource;
		entity.UpdatedAt = DateTime.UtcNow;
	}

	public ResponseModel MapToResponse(ProductViewHistory entity)
	{
		return new ResponseModel
		{
			Id = entity.Id,
			UserId = entity.UserId,
			ProductId = entity.ProductId,
			ViewedAt = entity.ViewedAt,
			ViewSource = entity.ViewSource,
			UpdatedAt = entity.UpdatedAt
		};
	}
}
