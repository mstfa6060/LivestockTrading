using LivestockTrading.Domain.Entities;

namespace LivestockTrading.Application.RequestHandlers.ProductVideos.Queries.Detail;

public class Mapper
{
	public ResponseModel MapToResponse(ProductVideo entity)
	{
		return new ResponseModel
		{
			Id = entity.Id,
			ProductId = entity.ProductId,
			VideoUrl = entity.VideoUrl,
			ThumbnailUrl = entity.ThumbnailUrl,
			Title = entity.Title,
			DurationSeconds = entity.DurationSeconds,
			SortOrder = entity.SortOrder,
			Provider = (int)entity.Provider,
			CreatedAt = entity.CreatedAt,
			UpdatedAt = entity.UpdatedAt
		};
	}
}
