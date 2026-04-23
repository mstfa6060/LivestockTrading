using LivestockTrading.Domain.Entities;

namespace LivestockTrading.Application.RequestHandlers.VeterinaryInfos.Queries.Pick;

public class Mapper
{
	public List<ResponseModel> MapToResponse(List<VeterinaryInfo> items)
	{
		return items.Select(e => new ResponseModel
		{
			Id = e.Id,
			TherapeuticCategory = e.TherapeuticCategory,
			Type = (int)e.Type
		}).ToList();
	}
}
