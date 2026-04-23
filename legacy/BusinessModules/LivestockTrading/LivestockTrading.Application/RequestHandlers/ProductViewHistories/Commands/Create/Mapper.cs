using LivestockTrading.Domain.Entities;

namespace LivestockTrading.Application.RequestHandlers.ProductViewHistories.Commands.Create;

public class Mapper
{
	public ProductViewHistory MapToEntity(RequestModel request)
	{
		return new ProductViewHistory
		{
			Id = Guid.NewGuid(),
			UserId = request.UserId,
			ProductId = request.ProductId,
			ViewedAt = DateTime.UtcNow,
			ViewSource = request.ViewSource,
			CreatedAt = DateTime.UtcNow
		};
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
			CreatedAt = entity.CreatedAt
		};
	}
}
