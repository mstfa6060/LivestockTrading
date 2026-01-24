using LivestockTrading.Domain.Entities;

namespace LivestockTrading.Application.RequestHandlers.ProductVideos.Commands.Create;

public class Mapper
{
	public ProductVideo MapToEntity(RequestModel request)
	{
		return new ProductVideo
		{
			Id = Guid.NewGuid(),
			ProductId = request.ProductId,
			VideoUrl = request.VideoUrl,
			ThumbnailUrl = request.ThumbnailUrl,
			Title = request.Title,
			DurationSeconds = request.DurationSeconds,
			SortOrder = request.SortOrder,
			Provider = (VideoProvider)request.Provider,
			CreatedAt = DateTime.UtcNow
		};
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
			CreatedAt = entity.CreatedAt
		};
	}
}
