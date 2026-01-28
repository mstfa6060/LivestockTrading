using LivestockTrading.Domain.Entities;

namespace LivestockTrading.Application.RequestHandlers.Transporters.Commands.Verify;

public class Mapper
{
	public ResponseModel MapToResponse(Transporter entity)
	{
		return new ResponseModel
		{
			Success = true,
			TransporterId = entity.Id,
			VerifiedAt = entity.VerifiedAt ?? DateTime.UtcNow
		};
	}
}
