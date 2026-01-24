namespace LivestockTrading.Application.RequestHandlers.SearchHistories.Queries.Detail;

public class RequestModel : IRequestModel
{
	public Guid Id { get; set; }
}

public class ResponseModel : IResponseModel
{
	public Guid Id { get; set; }
	public Guid UserId { get; set; }
	public string SearchQuery { get; set; }
	public string Filters { get; set; }
	public int ResultsCount { get; set; }
	public DateTime SearchedAt { get; set; }
	public DateTime CreatedAt { get; set; }
}
