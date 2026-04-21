using LivestockTrading.Domain.Entities;

namespace LivestockTrading.Application.RequestHandlers.Products.Commands.Approve;

public class Mapper
{
	public ResponseModel MapToResponse(Product entity)
	{
		return new ResponseModel
		{
			Success = true,
			ProductId = entity.Id,
			NewStatus = (int)entity.Status,
			ApprovedAt = entity.UpdatedAt ?? DateTime.UtcNow
		};
	}
}
