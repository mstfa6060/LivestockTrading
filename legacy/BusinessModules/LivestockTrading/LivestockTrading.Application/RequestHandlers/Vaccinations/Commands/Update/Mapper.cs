using LivestockTrading.Domain.Entities;

namespace LivestockTrading.Application.RequestHandlers.Vaccinations.Commands.Update;

public class Mapper
{
	public void MapToEntity(RequestModel request, Vaccination entity)
	{
		entity.AnimalInfoId = request.AnimalInfoId;
		entity.VaccineName = request.VaccineName;
		entity.VaccineType = request.VaccineType;
		entity.BatchNumber = request.BatchNumber;
		entity.VaccinationDate = request.VaccinationDate;
		entity.NextDueDate = request.NextDueDate;
		entity.VeterinarianName = request.VeterinarianName;
		entity.VeterinarianLicense = request.VeterinarianLicense;
		entity.Notes = request.Notes;
		entity.CertificateUrl = request.CertificateUrl;
		entity.UpdatedAt = DateTime.UtcNow;
	}

	public ResponseModel MapToResponse(Vaccination entity)
	{
		return new ResponseModel
		{
			Id = entity.Id,
			AnimalInfoId = entity.AnimalInfoId,
			VaccineName = entity.VaccineName,
			VaccineType = entity.VaccineType,
			BatchNumber = entity.BatchNumber,
			VaccinationDate = entity.VaccinationDate,
			NextDueDate = entity.NextDueDate,
			VeterinarianName = entity.VeterinarianName,
			VeterinarianLicense = entity.VeterinarianLicense,
			Notes = entity.Notes,
			CertificateUrl = entity.CertificateUrl,
			UpdatedAt = entity.UpdatedAt
		};
	}
}
