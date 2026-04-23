using LivestockTrading.Domain.Entities;

namespace LivestockTrading.Application.RequestHandlers.Vaccinations.Commands.Create;

public class Mapper
{
	public Vaccination MapToEntity(RequestModel request)
	{
		return new Vaccination
		{
			Id = Guid.NewGuid(),
			AnimalInfoId = request.AnimalInfoId,
			VaccineName = request.VaccineName,
			VaccineType = request.VaccineType,
			BatchNumber = request.BatchNumber,
			VaccinationDate = request.VaccinationDate,
			NextDueDate = request.NextDueDate,
			VeterinarianName = request.VeterinarianName,
			VeterinarianLicense = request.VeterinarianLicense,
			Notes = request.Notes,
			CertificateUrl = request.CertificateUrl,
			CreatedAt = DateTime.UtcNow
		};
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
			CreatedAt = entity.CreatedAt
		};
	}
}
