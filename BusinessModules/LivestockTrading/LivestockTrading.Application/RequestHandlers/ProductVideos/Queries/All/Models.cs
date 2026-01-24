namespace LivestockTrading.Application.RequestHandlers.ProductVideos.Queries.All;

public class RequestModel : IRequestModel
{
	public XSorting Sorting { get; set; }
	public List<XFilterItem> Filters { get; set; }
	public XPageRequest PageRequest { get; set; }
}

public class ResponseModel : IResponseModel<Array>
{
	public Guid Id { get; set; }
	public Guid ProductId { get; set; }
	public string VideoUrl { get; set; }
	public string ThumbnailUrl { get; set; }
	public string Title { get; set; }
	public int DurationSeconds { get; set; }
	public int SortOrder { get; set; }
	public int Provider { get; set; }
	public DateTime CreatedAt { get; set; }
}
