namespace BaseModules.IAM.Application.RequestHandlers.Provinces.Queries.All;

public class Mapper
{
    public List<ResponseModel> MapToResponse(List<Province> provinces)
    {
        return provinces.Select(p => new ResponseModel
        {
            Id = p.Id,
            Name = p.Name,
            Code = p.Code
        }).ToList();
    }
}
