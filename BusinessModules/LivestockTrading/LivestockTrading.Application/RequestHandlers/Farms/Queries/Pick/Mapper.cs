using LivestockTrading.Domain.Entities;

namespace LivestockTrading.Application.RequestHandlers.Farms.Queries.Pick;

public class Mapper
{
	public List<ResponseModel> MapToResponse(List<Farm> farms)
	{
		return farms.Select(f => new ResponseModel
		{
			Id = f.Id,
			Name = f.Name,
			RegistrationNumber = f.RegistrationNumber
		}).ToList();
	}
}
