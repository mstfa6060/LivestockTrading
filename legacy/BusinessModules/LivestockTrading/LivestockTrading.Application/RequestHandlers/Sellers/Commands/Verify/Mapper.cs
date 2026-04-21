using LivestockTrading.Domain.Entities;

namespace LivestockTrading.Application.RequestHandlers.Sellers.Commands.Verify;

public class Mapper
{
	public ResponseModel MapToResponse(Seller entity)
	{
		return new ResponseModel
		{
			Success = true,
			SellerId = entity.Id,
			VerifiedAt = entity.VerifiedAt ?? DateTime.UtcNow
		};
	}
}
