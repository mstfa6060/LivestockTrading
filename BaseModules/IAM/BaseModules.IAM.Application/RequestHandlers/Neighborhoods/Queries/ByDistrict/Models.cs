namespace BaseModules.IAM.Application.RequestHandlers.Neighborhoods.Queries.ByDistrict;

public class ResponseModel : IResponseModel<Array>
{
    public int Id { get; set; }
    public string Name { get; set; }
    public int DistrictId { get; set; }
    public string PostalCode { get; set; }
}

public class RequestModel : IRequestModel
{
    public int DistrictId { get; set; }
    public string Keyword { get; set; }
}
