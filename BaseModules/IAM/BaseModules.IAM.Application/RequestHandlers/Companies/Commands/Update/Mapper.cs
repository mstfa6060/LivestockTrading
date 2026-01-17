namespace BaseModules.IAM.Application.RequestHandlers.Companies.Commands.Update;

public class Mapper
{
	public ResponseModel MapToResponse(Company company)
	{
		return new ResponseModel
		{
			Id = company.Id
		};
	}
}
