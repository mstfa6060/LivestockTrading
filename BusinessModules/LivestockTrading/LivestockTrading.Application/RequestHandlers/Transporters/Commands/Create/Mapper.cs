using LivestockTrading.Domain.Entities;

namespace LivestockTrading.Application.RequestHandlers.Transporters.Commands.Create;

public class Mapper
{
	public Transporter MapToEntity(RequestModel request)
	{
		return new Transporter
		{
			Id = Guid.NewGuid(),
			UserId = request.UserId,
			CompanyName = request.CompanyName,
			ContactPerson = request.ContactPerson,
			Email = request.Email,
			Phone = request.Phone,
			Address = request.Address,
			City = request.City,
			CountryCode = request.CountryCode,
			LogoUrl = request.LogoUrl,
			Description = request.Description,
			LicenseNumber = request.LicenseNumber,
			TaxNumber = request.TaxNumber,
			InsuranceInfo = request.InsuranceInfo,
			FleetInfo = request.FleetInfo,
			ServiceRegions = request.ServiceRegions,
			Specializations = request.Specializations,
			IsActive = request.IsActive,
			Status = TransporterStatus.Active,
			IsVerified = true,
			VerifiedAt = DateTime.UtcNow,
			Website = request.Website,
			Certifications = request.Certifications,
			DocumentUrls = request.DocumentUrls,
			CreatedAt = DateTime.UtcNow
		};
	}

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
			IsActive = entity.IsActive,
			Website = entity.Website,
			Certifications = entity.Certifications,
			DocumentUrls = entity.DocumentUrls,
			CreatedAt = entity.CreatedAt
		};
	}
}
