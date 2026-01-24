using LivestockTrading.Domain.Entities;

namespace LivestockTrading.Application.RequestHandlers.Vaccinations.Queries.Detail;

public class Mapper
{
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
			CreatedAt = entity.CreatedAt,
			UpdatedAt = entity.UpdatedAt
		};
	}
}
