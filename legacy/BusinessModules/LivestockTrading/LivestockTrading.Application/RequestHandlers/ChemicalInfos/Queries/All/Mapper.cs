using LivestockTrading.Domain.Entities;

namespace LivestockTrading.Application.RequestHandlers.ChemicalInfos.Queries.All;

public class Mapper
{
	public List<ResponseModel> MapToResponse(List<ChemicalInfo> items)
	{
		return items.Select(e => new ResponseModel
		{
			Id = e.Id,
			ProductId = e.ProductId,
			Type = (int)e.Type,
			SubType = e.SubType,
			RegistrationNumber = e.RegistrationNumber,
			ToxicityLevel = (int)e.ToxicityLevel,
			IsOrganic = e.IsOrganic,
			ExpiryDate = e.ExpiryDate,
			CreatedAt = e.CreatedAt
		}).ToList();
	}
}
