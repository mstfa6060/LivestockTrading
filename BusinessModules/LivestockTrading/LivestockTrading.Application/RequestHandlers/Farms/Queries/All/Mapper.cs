using LivestockTrading.Domain.Entities;

namespace LivestockTrading.Application.RequestHandlers.Farms.Queries.All;

public class Mapper
{
	public List<ResponseModel> MapToResponse(List<Farm> farms)
	{
		return farms.Select(f => new ResponseModel
		{
			Id = f.Id,
			Name = f.Name,
			SellerId = f.SellerId,
			LocationId = f.LocationId,
			Type = (int)f.Type,
			TotalAreaHectares = f.TotalAreaHectares,
			IsOrganic = f.IsOrganic,
			IsActive = f.IsActive,
			IsVerified = f.IsVerified,
			CreatedAt = f.CreatedAt
		}).ToList();
	}
}
