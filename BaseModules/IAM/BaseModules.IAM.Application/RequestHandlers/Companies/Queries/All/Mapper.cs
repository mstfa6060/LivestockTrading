namespace BaseModules.IAM.Application.RequestHandlers.Companies.Queries.All;

public class Mapper
{
	public List<ResponseModel> MapToResponse(List<Company> companies)
	{
		return companies.Select(company => new ResponseModel
		{
			Id = company.Id,
			Name = company.Name,
			TaxNumber = company.TaxNumber,
			Address = company.Address,
			Phone = company.Phone
		}).ToList();
	}
}
