namespace BaseModules.IAM.Application.RequestHandlers.Companies.Commands.Create;

public class Mapper
{
	public Company MapToEntity(RequestModel payload)
	{
		return new Company
		{
			Id = Guid.NewGuid(),
			Name = payload.Name,
			TaxNumber = payload.TaxNumber,
			Address = payload.Address,
			Phone = payload.Phone,
			CreatedAt = DateTime.UtcNow
		};
	}

	public ResponseModel MapToResponse(Company company)
	{
		return new ResponseModel
		{
			Id = company.Id
		};
	}
}
