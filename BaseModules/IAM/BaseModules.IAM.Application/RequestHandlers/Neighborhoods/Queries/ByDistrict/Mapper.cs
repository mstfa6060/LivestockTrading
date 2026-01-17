namespace BaseModules.IAM.Application.RequestHandlers.Neighborhoods.Queries.ByDistrict;

public class Mapper
{
    public List<ResponseModel> MapToResponse(List<Neighborhood> neighborhoods)
    {
        return neighborhoods.Select(n => new ResponseModel
        {
            Id = n.Id,
            Name = n.Name,
            DistrictId = n.DistrictId,
            PostalCode = n.PostalCode
        }).ToList();
    }
}
