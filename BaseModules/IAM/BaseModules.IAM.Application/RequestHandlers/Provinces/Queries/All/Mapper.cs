namespace BaseModules.IAM.Application.RequestHandlers.Provinces.Queries.All;

public class Mapper
{
    public List<ResponseModel> MapToResponse(List<Province> provinces)
    {
        return provinces.Select(p => new ResponseModel
        {
            Id = p.Id,
            CountryId = p.CountryId,
            Name = p.Name,
            Code = p.Code,
            NameTranslations = p.NameTranslations
        }).ToList();
    }
}
