namespace BaseModules.IAM.Application.RequestHandlers.Districts.Queries.ByProvince;

public class ResponseModel : IResponseModel<Array>
{
    public int Id { get; set; }
    public string Name { get; set; }
    public int ProvinceId { get; set; }
    public string NameTranslations { get; set; }
}

public class RequestModel : IRequestModel
{
    public int ProvinceId { get; set; }
    public string Keyword { get; set; }
}
