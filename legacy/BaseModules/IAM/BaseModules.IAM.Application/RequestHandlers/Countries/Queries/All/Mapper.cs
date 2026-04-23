namespace BaseModules.IAM.Application.RequestHandlers.Countries.Queries.All;

public class Mapper
{
    public List<ResponseModel> MapToResponse(List<Country> countries)
    {
        return countries.Select(c => new ResponseModel
        {
            Id = c.Id,
            Code = c.Code,
            Code3 = c.Code3,
            Name = c.Name,
            NativeName = c.NativeName,
            PhoneCode = c.PhoneCode,
            DefaultCurrencyCode = c.DefaultCurrencyCode,
            DefaultCurrencySymbol = c.DefaultCurrencySymbol
        }).ToList();
    }
}
