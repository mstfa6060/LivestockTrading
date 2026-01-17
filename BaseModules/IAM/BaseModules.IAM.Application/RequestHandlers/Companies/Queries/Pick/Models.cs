namespace BaseModules.IAM.Application.RequestHandlers.Companies.Queries.Pick;

public class RequestModel : IRequestModel
{
    public List<Guid> SelectedIds { get; set; }
    public string Keyword { get; set; }
    public int Limit { get; set; }
}

public class ResponseModel : IResponseModel<Array>
{
    public Guid Id { get; set; }
    public string Name { get; set; }
}
