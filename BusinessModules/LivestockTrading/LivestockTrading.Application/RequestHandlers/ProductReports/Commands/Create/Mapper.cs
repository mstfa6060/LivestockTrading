using LivestockTrading.Domain.Entities;

namespace LivestockTrading.Application.RequestHandlers.ProductReports.Commands.Create;

public class Mapper
{
	public ProductReport MapToEntity(RequestModel request, Guid reporterUserId)
	{
		return new ProductReport
		{
			Id = Guid.NewGuid(),
			ProductId = request.ProductId,
			ReporterUserId = reporterUserId,
			Reason = request.Reason,
			Description = request.Description,
			Status = 0, // Pending
			CreatedAt = DateTime.UtcNow
		};
	}
}
