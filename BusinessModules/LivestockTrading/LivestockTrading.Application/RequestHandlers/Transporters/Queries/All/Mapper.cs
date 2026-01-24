using LivestockTrading.Domain.Entities;

namespace LivestockTrading.Application.RequestHandlers.Transporters.Queries.All;

public class Mapper
{
	public List<ResponseModel> MapToResponse(List<Transporter> transporters)
	{
		return transporters.Select(t => new ResponseModel
		{
			Id = t.Id,
			UserId = t.UserId,
			CompanyName = t.CompanyName,
			ContactPerson = t.ContactPerson,
			Email = t.Email,
			Phone = t.Phone,
			City = t.City,
			CountryCode = t.CountryCode,
			IsVerified = t.IsVerified,
			IsActive = t.IsActive,
			AverageRating = t.AverageRating,
			TotalTransports = t.TotalTransports,
			CreatedAt = t.CreatedAt
		}).ToList();
	}
}
