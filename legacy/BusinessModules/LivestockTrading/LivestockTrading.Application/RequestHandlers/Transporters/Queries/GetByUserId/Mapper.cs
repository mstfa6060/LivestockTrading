using LivestockTrading.Domain.Entities;

namespace LivestockTrading.Application.RequestHandlers.Transporters.Queries.GetByUserId;

public class Mapper
{
	public ResponseModel MapToResponse(Transporter entity)
	{
		return new ResponseModel
		{
			Id = entity.Id,
			UserId = entity.UserId,
			CompanyName = entity.CompanyName,
			ContactPerson = entity.ContactPerson,
			Email = entity.Email,
			Phone = entity.Phone,
			Address = entity.Address,
			City = entity.City,
			CountryCode = entity.CountryCode,
			LogoUrl = entity.LogoUrl,
			Description = entity.Description,
			ServiceRegions = entity.ServiceRegions,
			Specializations = entity.Specializations,
			IsVerified = entity.IsVerified,
			IsActive = entity.IsActive,
			Status = (int)entity.Status,
			AverageRating = entity.AverageRating,
			TotalTransports = entity.TotalTransports,
			CompletedTransports = entity.CompletedTransports,
			Website = entity.Website,
			CreatedAt = entity.CreatedAt
		};
	}
}
