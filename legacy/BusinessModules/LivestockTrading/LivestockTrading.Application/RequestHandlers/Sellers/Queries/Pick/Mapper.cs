using LivestockTrading.Domain.Entities;

namespace LivestockTrading.Application.RequestHandlers.Sellers.Queries.Pick;

public class Mapper
{
	public List<ResponseModel> MapToResponse(List<Seller> sellers)
	{
		return sellers.Select(s => new ResponseModel
		{
			Id = s.Id,
			BusinessName = s.BusinessName,
			Email = s.Email
		}).ToList();
	}
}
