using LivestockTrading.Domain.Entities;

namespace LivestockTrading.Application.RequestHandlers.Transporters.Queries.Detail;

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
			LicenseNumber = entity.LicenseNumber,
			TaxNumber = entity.TaxNumber,
			InsuranceInfo = entity.InsuranceInfo,
			FleetInfo = entity.FleetInfo,
			ServiceRegions = entity.ServiceRegions,
			Specializations = entity.Specializations,
			IsVerified = entity.IsVerified,
			VerifiedAt = entity.VerifiedAt,
			IsActive = entity.IsActive,
			AverageRating = entity.AverageRating,
			TotalTransports = entity.TotalTransports,
			CompletedTransports = entity.CompletedTransports,
			Website = entity.Website,
			Certifications = entity.Certifications,
			DocumentUrls = entity.DocumentUrls,
			CreatedAt = entity.CreatedAt,
			UpdatedAt = entity.UpdatedAt
		};
	}
}
