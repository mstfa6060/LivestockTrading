using LivestockTrading.Domain.Entities;

namespace LivestockTrading.Application.RequestHandlers.AnimalInfos.Queries.Pick;

public class Mapper
{
	public List<ResponseModel> MapToResponse(List<AnimalInfo> items)
	{
		return items.Select(e => new ResponseModel
		{
			Id = e.Id,
			BreedName = e.BreedName,
			TagNumber = e.TagNumber
		}).ToList();
	}
}
