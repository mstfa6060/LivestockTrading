namespace BaseModules.IAM.Application.RequestHandlers.Companies.Commands.Delete;

public class Mapper
{
	public ResponseModel MapToResponse(Company company)
	{
		return new ResponseModel
		{
			Id = company.Id,
			IsDeleted = company.IsDeleted
		};
	}
}
