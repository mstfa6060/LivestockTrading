namespace BaseModules.IAM.Application.RequestHandlers.Countries.Queries.All;

public class ResponseModel : IResponseModel<Array>
{
    public int Id { get; set; }
    public string Code { get; set; }
    public string Code3 { get; set; }
    public string Name { get; set; }
    public string NativeName { get; set; }
    public string PhoneCode { get; set; }
    public string DefaultCurrencyCode { get; set; }
    public string DefaultCurrencySymbol { get; set; }
}

public class RequestModel : IRequestModel
{
    public string Keyword { get; set; }
}
