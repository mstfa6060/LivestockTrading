using LivestockTrading.Domain.Entities;

namespace LivestockTrading.Application.RequestHandlers.SearchHistories.Queries.Pick;

public class Mapper
{
	public List<ResponseModel> MapToResponse(List<SearchHistory> items)
	{
		return items.Select(entity => new ResponseModel
		{
			Id = entity.Id,
			SearchQuery = entity.SearchQuery,
			SearchedAt = entity.SearchedAt
		}).ToList();
	}
}
