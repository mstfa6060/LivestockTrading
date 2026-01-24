using LivestockTrading.Domain.Entities;

namespace LivestockTrading.Application.RequestHandlers.Transporters.Commands.Update;

public class Mapper
{
	public void MapToEntity(RequestModel request, Transporter entity)
	{
		entity.UserId = request.UserId;
		entity.CompanyName = request.CompanyName;
		entity.ContactPerson = request.ContactPerson;
		entity.Email = request.Email;
		entity.Phone = request.Phone;
		entity.Address = request.Address;
		entity.City = request.City;
		entity.CountryCode = request.CountryCode;
		entity.LogoUrl = request.LogoUrl;
		entity.Description = request.Description;
		entity.LicenseNumber = request.LicenseNumber;
		entity.TaxNumber = request.TaxNumber;
		entity.InsuranceInfo = request.InsuranceInfo;
		entity.FleetInfo = request.FleetInfo;
		entity.ServiceRegions = request.ServiceRegions;
		entity.Specializations = request.Specializations;
		entity.IsActive = request.IsActive;
		entity.Website = request.Website;
		entity.Certifications = request.Certifications;
		entity.DocumentUrls = request.DocumentUrls;
		entity.UpdatedAt = DateTime.UtcNow;
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
			UpdatedAt = entity.UpdatedAt
		};
	}
}
