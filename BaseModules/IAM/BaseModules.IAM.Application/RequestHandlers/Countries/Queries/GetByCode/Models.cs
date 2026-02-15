namespace BaseModules.IAM.Application.RequestHandlers.Countries.Queries.GetByCode;

public class RequestModel : IRequestModel
{
    public string Code { get; set; }
}

public class ResponseModel : IResponseModel
{
    public int Id { get; set; }
    public string Code { get; set; }
    public string Code3 { get; set; }
    public int NumericCode { get; set; }
    public string Name { get; set; }
    public string NativeName { get; set; }
    public string Capital { get; set; }
    public string Continent { get; set; }
    public string Region { get; set; }
    public string PhoneCode { get; set; }
    public string DefaultCurrencyCode { get; set; }
    public string DefaultCurrencySymbol { get; set; }
    public string Flag { get; set; }
    public string Timezone { get; set; }
    public string DefaultLanguage { get; set; }
    public bool IsActive { get; set; }
}
