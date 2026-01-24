using LivestockTrading.Domain.Entities;

namespace LivestockTrading.Application.RequestHandlers.ChemicalInfos.Queries.Pick;

public class Mapper
{
	public List<ResponseModel> MapToResponse(List<ChemicalInfo> items)
	{
		return items.Select(e => new ResponseModel
		{
			Id = e.Id,
			SubType = e.SubType,
			RegistrationNumber = e.RegistrationNumber
		}).ToList();
	}
}
