using LivestockTrading.Domain.Entities;

namespace LivestockTrading.Application.RequestHandlers.SearchHistories.Queries.All;

public class Mapper
{
	public List<ResponseModel> MapToResponse(List<SearchHistory> items)
	{
		return items.Select(entity => new ResponseModel
		{
			Id = entity.Id,
			UserId = entity.UserId,
			SearchQuery = entity.SearchQuery,
			Filters = entity.Filters,
			ResultsCount = entity.ResultsCount,
			SearchedAt = entity.SearchedAt,
			CreatedAt = entity.CreatedAt
		}).ToList();
	}
}
