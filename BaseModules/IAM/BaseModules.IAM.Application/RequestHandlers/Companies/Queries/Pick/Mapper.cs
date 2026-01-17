namespace BaseModules.IAM.Application.RequestHandlers.Companies.Queries.Pick;

public class Mapper
{
	public List<ResponseModel> MapToResponse(List<Company> companies)
	{
		return companies.Select(x => new ResponseModel
		{
			Id = x.Id,
			Name = x.Name
		}).ToList();
	}
}
