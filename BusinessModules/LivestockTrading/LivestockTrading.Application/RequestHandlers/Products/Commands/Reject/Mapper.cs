using LivestockTrading.Domain.Entities;

namespace LivestockTrading.Application.RequestHandlers.Products.Commands.Reject;

public class Mapper
{
	public ResponseModel MapToResponse(Product entity, string rejectionReason)
	{
		return new ResponseModel
		{
			Success = true,
			ProductId = entity.Id,
			NewStatus = (int)entity.Status,
			RejectionReason = rejectionReason,
			RejectedAt = entity.UpdatedAt ?? DateTime.UtcNow
		};
	}
}
