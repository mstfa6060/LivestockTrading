using LivestockTrading.Domain.Entities;

namespace LivestockTrading.Application.RequestHandlers.ProductVideos.Queries.All;

public class Mapper
{
	public List<ResponseModel> MapToResponse(List<ProductVideo> items)
	{
		return items.Select(e => new ResponseModel
		{
			Id = e.Id,
			ProductId = e.ProductId,
			VideoUrl = e.VideoUrl,
			ThumbnailUrl = e.ThumbnailUrl,
			Title = e.Title,
			DurationSeconds = e.DurationSeconds,
			SortOrder = e.SortOrder,
			Provider = (int)e.Provider,
			CreatedAt = e.CreatedAt
		}).ToList();
	}
}
