using LivestockTrading.Domain.Entities;

namespace LivestockTrading.Application.RequestHandlers.ProductVideos.Commands.Update;

public class Mapper
{
	public void MapToEntity(RequestModel request, ProductVideo entity)
	{
		entity.VideoUrl = request.VideoUrl;
		entity.ThumbnailUrl = request.ThumbnailUrl;
		entity.Title = request.Title;
		entity.DurationSeconds = request.DurationSeconds;
		entity.SortOrder = request.SortOrder;
		entity.Provider = (VideoProvider)request.Provider;
		entity.UpdatedAt = DateTime.UtcNow;
	}

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
			UpdatedAt = entity.UpdatedAt
		};
	}
}
