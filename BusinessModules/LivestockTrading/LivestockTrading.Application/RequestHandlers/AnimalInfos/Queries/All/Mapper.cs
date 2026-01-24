using LivestockTrading.Domain.Entities;

namespace LivestockTrading.Application.RequestHandlers.AnimalInfos.Queries.All;

public class Mapper
{
	public List<ResponseModel> MapToResponse(List<AnimalInfo> items)
	{
		return items.Select(e => new ResponseModel
		{
			Id = e.Id,
			ProductId = e.ProductId,
			BreedName = e.BreedName,
			Gender = (int)e.Gender,
			AgeMonths = e.AgeMonths,
			WeightKg = e.WeightKg,
			Color = e.Color,
			TagNumber = e.TagNumber,
			HealthStatus = (int)e.HealthStatus,
			Purpose = (int)e.Purpose,
			CreatedAt = e.CreatedAt
		}).ToList();
	}
}
