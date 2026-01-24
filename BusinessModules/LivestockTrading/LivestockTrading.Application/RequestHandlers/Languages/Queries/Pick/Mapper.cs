using LivestockTrading.Domain.Entities;

namespace LivestockTrading.Application.RequestHandlers.Languages.Queries.Pick;

public class Mapper
{
	public List<ResponseModel> MapToResponse(List<Language> languages)
	{
		return languages.Select(l => new ResponseModel
		{
			Id = l.Id,
			Code = l.Code,
			Name = l.Name,
			NativeName = l.NativeName
		}).ToList();
	}
}
