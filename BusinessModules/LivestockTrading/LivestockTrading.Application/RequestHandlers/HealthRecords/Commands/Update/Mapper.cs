using LivestockTrading.Domain.Entities;

namespace LivestockTrading.Application.RequestHandlers.HealthRecords.Commands.Update;

public class Mapper
{
	public void MapToEntity(RequestModel request, HealthRecord entity)
	{
		entity.AnimalInfoId = request.AnimalInfoId;
		entity.RecordDate = request.RecordDate;
		entity.RecordType = request.RecordType;
		entity.VeterinarianName = request.VeterinarianName;
		entity.VeterinarianLicense = request.VeterinarianLicense;
		entity.ClinicName = request.ClinicName;
		entity.Diagnosis = request.Diagnosis;
		entity.Treatment = request.Treatment;
		entity.Medications = request.Medications;
		entity.Notes = request.Notes;
		entity.FollowUpDate = request.FollowUpDate;
		entity.DocumentUrl = request.DocumentUrl;
		entity.UpdatedAt = DateTime.UtcNow;
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
			UpdatedAt = entity.UpdatedAt
		};
	}
}
