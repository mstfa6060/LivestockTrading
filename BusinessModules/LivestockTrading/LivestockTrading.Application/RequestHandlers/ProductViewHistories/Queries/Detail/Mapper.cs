using LivestockTrading.Domain.Entities;

namespace LivestockTrading.Application.RequestHandlers.ProductViewHistories.Queries.Detail;

public class Mapper
{
	public ResponseModel MapToResponse(ProductViewHistory entity)
	{
		return new ResponseModel
		{
			Id = entity.Id,
			UserId = entity.UserId,
			ProductId = entity.ProductId,
			ViewedAt = entity.ViewedAt,
			ViewSource = entity.ViewSource,
			CreatedAt = entity.CreatedAt,
			UpdatedAt = entity.UpdatedAt
		};
	}
}
