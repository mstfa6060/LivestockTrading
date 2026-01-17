namespace BaseModules.IAM.Application.RequestHandlers.Companies.Queries.Detail;

public class Mapper
{
    public ResponseModel MapToResponse(Company company)
    {
        return new ResponseModel
        {
            Id = company.Id,
            Name = company.Name,
            TaxNumber = company.TaxNumber,
            Address = company.Address,
            Phone = company.Phone
        };
    }
}
