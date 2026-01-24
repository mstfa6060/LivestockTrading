using LivestockTrading.Domain.Entities;

namespace LivestockTrading.Application.RequestHandlers.Vaccinations.Queries.All;

public class Mapper
{
	public List<ResponseModel> MapToResponse(List<Vaccination> items)
	{
		return items.Select(e => new ResponseModel
		{
			Id = e.Id,
			AnimalInfoId = e.AnimalInfoId,
			VaccineName = e.VaccineName,
			VaccineType = e.VaccineType,
			VaccinationDate = e.VaccinationDate,
			NextDueDate = e.NextDueDate,
			VeterinarianName = e.VeterinarianName,
			CreatedAt = e.CreatedAt
		}).ToList();
	}
}
