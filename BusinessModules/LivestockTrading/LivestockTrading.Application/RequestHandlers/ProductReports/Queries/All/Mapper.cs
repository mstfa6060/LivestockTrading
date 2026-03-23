using LivestockTrading.Domain.Entities;

namespace LivestockTrading.Application.RequestHandlers.ProductReports.Queries.All;

public class Mapper
{
	public List<ResponseModel> MapToResponse(List<ProductReport> reports)
	{
		return reports.Select(r => new ResponseModel
		{
			Id = r.Id,
			ProductId = r.ProductId,
			ReporterUserId = r.ReporterUserId,
			Reason = r.Reason,
			Description = r.Description,
			Status = r.Status,
			AdminNote = r.AdminNote,
			ReviewedByUserId = r.ReviewedByUserId,
			ReviewedAt = r.ReviewedAt,
			CreatedAt = r.CreatedAt
		}).ToList();
	}
}
