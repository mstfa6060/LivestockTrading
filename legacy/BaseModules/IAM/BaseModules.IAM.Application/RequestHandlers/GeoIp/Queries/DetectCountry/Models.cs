namespace BaseModules.IAM.Application.RequestHandlers.GeoIp.Queries.DetectCountry;

public class ResponseModel : IResponseModel
{
    public string CountryCode { get; set; }
    public int? CountryId { get; set; }
    public string CountryName { get; set; }
}

public class RequestModel : IRequestModel
{
}
