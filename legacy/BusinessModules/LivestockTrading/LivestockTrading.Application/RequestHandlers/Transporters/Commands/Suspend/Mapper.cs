using LivestockTrading.Domain.Entities;

namespace LivestockTrading.Application.RequestHandlers.Transporters.Commands.Suspend;

public class Mapper
{
	public ResponseModel MapToResponse(Transporter entity, DateTime suspendedAt)
	{
		return new ResponseModel
		{
			Success = true,
			TransporterId = entity.Id,
			SuspendedAt = suspendedAt
		};
	}
}
