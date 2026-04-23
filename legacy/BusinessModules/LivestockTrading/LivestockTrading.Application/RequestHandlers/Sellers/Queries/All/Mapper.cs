using LivestockTrading.Domain.Entities;

namespace LivestockTrading.Application.RequestHandlers.Sellers.Queries.All;

public class Mapper
{
	public List<ResponseModel> MapToResponse(List<Seller> sellers)
	{
		return sellers.Select(s => new ResponseModel
		{
			Id = s.Id,
			UserId = s.UserId,
			BusinessName = s.BusinessName,
			BusinessType = s.BusinessType,
			Email = s.Email,
			Phone = s.Phone,
			IsVerified = s.IsVerified,
			IsActive = s.IsActive,
			Status = (int)s.Status,
			AverageRating = s.AverageRating,
			TotalReviews = s.TotalReviews,
			TotalSales = s.TotalSales,
			CreatedAt = s.CreatedAt
		}).ToList();
	}
}
