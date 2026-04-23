using LivestockTrading.Domain.Entities;

namespace LivestockTrading.Application.RequestHandlers.Transporters.Queries.Pick;

public class Mapper
{
	public List<ResponseModel> MapToResponse(List<Transporter> transporters)
	{
		return transporters.Select(t => new ResponseModel
		{
			Id = t.Id,
			CompanyName = t.CompanyName,
			City = t.City
		}).ToList();
	}
}
