using LivestockTrading.Domain.Entities;

namespace LivestockTrading.Application.RequestHandlers.UserPreferences.Queries.Pick;

public class Mapper
{
	public List<ResponseModel> MapToResponse(List<Domain.Entities.UserPreferences> items)
	{
		return items.Select(e => new ResponseModel
		{
			Id = e.Id,
			UserId = e.UserId,
			PreferredCurrency = e.PreferredCurrency
		}).ToList();
	}
}
