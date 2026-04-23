using LivestockTrading.Domain.Entities;

namespace LivestockTrading.Application.RequestHandlers.SearchHistories.Commands.Update;

public class Mapper
{
	public void MapToEntity(RequestModel request, SearchHistory entity)
	{
		entity.SearchQuery = request.SearchQuery;
		entity.UpdatedAt = DateTime.UtcNow;
	}

	public ResponseModel MapToResponse(SearchHistory entity)
	{
		return new ResponseModel
		{
			Id = entity.Id,
			UserId = entity.UserId,
			SearchQuery = entity.SearchQuery,
			Filters = entity.Filters,
			ResultsCount = entity.ResultsCount,
			SearchedAt = entity.SearchedAt,
			UpdatedAt = entity.UpdatedAt
		};
	}
}
