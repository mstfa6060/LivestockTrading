using LivestockTrading.Domain.Entities;

namespace LivestockTrading.Application.RequestHandlers.Products.Commands.Reject;

public class Mapper
{
	public ResponseModel MapToResponse(Product entity, string reason)
	{
		return new ResponseModel
		{
			Success = true,
			ProductId = entity.Id,
			NewStatus = (int)entity.Status,
			RejectionReason = reason,
			RejectedAt = entity.UpdatedAt ?? DateTime.UtcNow
		};
	}
}
