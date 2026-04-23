using LivestockTrading.Domain.Entities;

namespace LivestockTrading.Application.RequestHandlers.ProductReports.Commands.Update;

public class Mapper
{
	public void MapToEntity(RequestModel request, ProductReport entity, Guid reviewedByUserId)
	{
		entity.Status = request.Status;
		entity.AdminNote = request.AdminNote;
		entity.ReviewedByUserId = reviewedByUserId;
		entity.ReviewedAt = DateTime.UtcNow;
		entity.UpdatedAt = DateTime.UtcNow;
	}

	public ResponseModel MapToResponse(ProductReport entity)
	{
		return new ResponseModel
		{
			Id = entity.Id,
			Status = entity.Status,
			AdminNote = entity.AdminNote,
			ReviewedByUserId = entity.ReviewedByUserId,
			ReviewedAt = entity.ReviewedAt,
			Success = true
		};
	}
}
