using LivestockTrading.Domain.Entities;

namespace LivestockTrading.Application.RequestHandlers.HealthRecords.Queries.All;

public class Mapper
{
	public List<ResponseModel> MapToResponse(List<HealthRecord> items)
	{
		return items.Select(e => new ResponseModel
		{
			Id = e.Id,
			AnimalInfoId = e.AnimalInfoId,
			RecordDate = e.RecordDate,
			RecordType = e.RecordType,
			VeterinarianName = e.VeterinarianName,
			VeterinarianLicense = e.VeterinarianLicense,
			ClinicName = e.ClinicName,
			Diagnosis = e.Diagnosis,
			Treatment = e.Treatment,
			Medications = e.Medications,
			Notes = e.Notes,
			FollowUpDate = e.FollowUpDate,
			DocumentUrl = e.DocumentUrl,
			CreatedAt = e.CreatedAt
		}).ToList();
	}
}
