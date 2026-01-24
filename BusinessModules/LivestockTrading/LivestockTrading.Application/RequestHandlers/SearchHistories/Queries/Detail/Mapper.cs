using LivestockTrading.Domain.Entities;

namespace LivestockTrading.Application.RequestHandlers.SearchHistories.Queries.Detail;

public class Mapper
{
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
