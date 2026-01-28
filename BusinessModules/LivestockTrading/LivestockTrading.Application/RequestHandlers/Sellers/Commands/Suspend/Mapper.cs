using LivestockTrading.Domain.Entities;

namespace LivestockTrading.Application.RequestHandlers.Sellers.Commands.Suspend;

public class Mapper
{
	public ResponseModel MapToResponse(Seller entity, DateTime suspendedAt)
	{
		return new ResponseModel
		{
			Success = true,
			SellerId = entity.Id,
			SuspendedAt = suspendedAt
		};
	}
}
