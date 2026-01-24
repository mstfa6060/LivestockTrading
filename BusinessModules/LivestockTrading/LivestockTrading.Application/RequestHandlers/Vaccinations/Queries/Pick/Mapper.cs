using LivestockTrading.Domain.Entities;

namespace LivestockTrading.Application.RequestHandlers.Vaccinations.Queries.Pick;

public class Mapper
{
	public List<ResponseModel> MapToResponse(List<Vaccination> items)
	{
		return items.Select(e => new ResponseModel
		{
			Id = e.Id,
			VaccineName = e.VaccineName,
			VaccinationDate = e.VaccinationDate
		}).ToList();
	}
}
