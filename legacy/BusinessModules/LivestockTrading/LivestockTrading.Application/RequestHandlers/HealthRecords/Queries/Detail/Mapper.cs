using LivestockTrading.Domain.Entities;

namespace LivestockTrading.Application.RequestHandlers.HealthRecords.Queries.Detail;

public class Mapper
{
	public ResponseModel MapToResponse(HealthRecord entity)
	{
		return new ResponseModel
		{
			Id = entity.Id,
			AnimalInfoId = entity.AnimalInfoId,
			RecordDate = entity.RecordDate,
			RecordType = entity.RecordType,
			VeterinarianName = entity.VeterinarianName,
			VeterinarianLicense = entity.VeterinarianLicense,
			ClinicName = entity.ClinicName,
			Diagnosis = entity.Diagnosis,
			Treatment = entity.Treatment,
			Medications = entity.Medications,
			Notes = entity.Notes,
			FollowUpDate = entity.FollowUpDate,
			DocumentUrl = entity.DocumentUrl,
			CreatedAt = entity.CreatedAt,
			UpdatedAt = entity.UpdatedAt
		};
	}
}
