namespace BaseModules.IAM.Application.RequestHandlers.Districts.Queries.ByProvince;

public class Mapper
{
    public List<ResponseModel> MapToResponse(List<District> districts)
    {
        return districts.Select(d => new ResponseModel
        {
            Id = d.Id,
            Name = d.Name,
            ProvinceId = d.ProvinceId,
            NameTranslations = d.NameTranslations
        }).ToList();
    }
}
