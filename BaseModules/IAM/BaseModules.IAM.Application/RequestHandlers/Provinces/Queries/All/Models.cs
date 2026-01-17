namespace BaseModules.IAM.Application.RequestHandlers.Provinces.Queries.All;

public class ResponseModel : IResponseModel<Array>
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Code { get; set; }
}

public class RequestModel : IRequestModel
{
    public string Keyword { get; set; }
}
