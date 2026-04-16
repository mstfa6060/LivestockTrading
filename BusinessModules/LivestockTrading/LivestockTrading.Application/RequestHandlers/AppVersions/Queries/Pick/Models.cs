namespace LivestockTrading.Application.RequestHandlers.AppVersions.Queries.Pick;

public class RequestModel : IRequestModel
{
	public List<Guid> SelectedIds { get; set; }
	public string Keyword { get; set; }
	public int Limit { get; set; } = 10;
}

public class ResponseModel : IResponseModel<Array>
{
	public Guid Id { get; set; }
	public int Platform { get; set; }
	public string LatestVersion { get; set; }
	public string UpdateMessage { get; set; }
}
