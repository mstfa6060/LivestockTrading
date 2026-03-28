namespace BaseModules.IAM.Application.RequestHandlers.Provinces.Queries.All;

public class ResponseModel : IResponseModel<Array>
{
    public int Id { get; set; }
    public int CountryId { get; set; }
    public string Name { get; set; }
    public string Code { get; set; }
    public string NameTranslations { get; set; }
}

public class RequestModel : IRequestModel
{
    public int CountryId { get; set; }
    public string Keyword { get; set; }
}
