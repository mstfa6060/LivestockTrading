using LivestockTrading.Domain.Entities;

namespace LivestockTrading.Application.RequestHandlers.Languages.Queries.All;

public class Mapper
{
	public List<ResponseModel> MapToResponse(List<Language> languages)
	{
		return languages.Select(l => new ResponseModel
		{
			Id = l.Id,
			Code = l.Code,
			Name = l.Name,
			NativeName = l.NativeName,
			IsRightToLeft = l.IsRightToLeft,
			IsActive = l.IsActive,
			IsDefault = l.IsDefault,
			SortOrder = l.SortOrder,
			FlagIconUrl = l.FlagIconUrl,
			CreatedAt = l.CreatedAt
		}).ToList();
	}
}
