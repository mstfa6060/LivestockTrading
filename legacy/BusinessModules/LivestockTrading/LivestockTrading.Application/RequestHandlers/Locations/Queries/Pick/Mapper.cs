using LivestockTrading.Domain.Entities;

namespace LivestockTrading.Application.RequestHandlers.Locations.Queries.Pick;

public class Mapper
{
	public List<ResponseModel> MapToResponse(List<Location> locations)
	{
		return locations.Select(l => new ResponseModel
		{
			Id = l.Id,
			Name = l.Name,
			City = l.City,
			CountryCode = l.CountryCode
		}).ToList();
	}
}
