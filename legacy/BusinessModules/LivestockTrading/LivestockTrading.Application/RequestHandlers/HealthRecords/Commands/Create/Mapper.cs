using LivestockTrading.Domain.Entities;

namespace LivestockTrading.Application.RequestHandlers.HealthRecords.Commands.Create;

public class Mapper
{
	public HealthRecord MapToEntity(RequestModel request)
	{
		return new HealthRecord
		{
			Id = Guid.NewGuid(),
			AnimalInfoId = request.AnimalInfoId,
			RecordDate = request.RecordDate,
			RecordType = request.RecordType,
			VeterinarianName = request.VeterinarianName,
			VeterinarianLicense = request.VeterinarianLicense,
			ClinicName = request.ClinicName,
			Diagnosis = request.Diagnosis,
			Treatment = request.Treatment,
			Medications = request.Medications,
			Notes = request.Notes,
			FollowUpDate = request.FollowUpDate,
			DocumentUrl = request.DocumentUrl,
			CreatedAt = DateTime.UtcNow
		};
	}

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
			CreatedAt = entity.CreatedAt
		};
	}
}
