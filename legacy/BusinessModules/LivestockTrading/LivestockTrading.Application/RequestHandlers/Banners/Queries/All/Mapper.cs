using LivestockTrading.Domain.Entities;

namespace LivestockTrading.Application.RequestHandlers.Banners.Queries.All;

public class Mapper
{
	public List<ResponseModel> MapToResponse(List<Banner> items)
	{
		return items.Select(e => new ResponseModel
		{
			Id = e.Id,
			Title = e.Title,
			Description = e.Description,
			ImageUrl = e.ImageUrl,
			TargetUrl = e.TargetUrl,
			Position = (int)e.Position,
			StartDate = e.StartDate,
			EndDate = e.EndDate,
			IsActive = e.IsActive,
			DisplayOrder = e.DisplayOrder,
			ClickCount = e.ClickCount,
			ImpressionCount = e.ImpressionCount,
			CreatedAt = e.CreatedAt
		}).ToList();
	}
}
