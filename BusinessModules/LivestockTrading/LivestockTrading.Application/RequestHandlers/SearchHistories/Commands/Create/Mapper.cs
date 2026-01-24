using LivestockTrading.Domain.Entities;

namespace LivestockTrading.Application.RequestHandlers.SearchHistories.Commands.Create;

public class Mapper
{
	public SearchHistory MapToEntity(RequestModel request)
	{
		return new SearchHistory
		{
			Id = Guid.NewGuid(),
			UserId = request.UserId,
			SearchQuery = request.SearchQuery,
			Filters = request.Filters,
			ResultsCount = request.ResultsCount,
			SearchedAt = DateTime.UtcNow,
			CreatedAt = DateTime.UtcNow
		};
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
			CreatedAt = entity.CreatedAt
		};
	}
}
