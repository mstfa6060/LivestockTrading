namespace BaseModules.IAM.Application.RequestHandlers.Geo.Queries.Detect;

public class RequestModel : IRequestModel
{
}

public class ResponseModel : IResponseModel
{
    public string CountryCode { get; set; }
    public string CountryName { get; set; }
    public string Currency { get; set; }
    public string Language { get; set; }
    public string Timezone { get; set; }
}
