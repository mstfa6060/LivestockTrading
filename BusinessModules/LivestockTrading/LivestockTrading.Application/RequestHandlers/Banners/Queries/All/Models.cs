namespace LivestockTrading.Application.RequestHandlers.Banners.Queries.All;

public class RequestModel : IRequestModel
{
	public XSorting Sorting { get; set; }
	public List<XFilterItem> Filters { get; set; }
	public XPageRequest PageRequest { get; set; }
}

public class ResponseModel : IResponseModel<Array>
{
	public Guid Id { get; set; }
	public string Title { get; set; }
	public string Description { get; set; }
	public string ImageUrl { get; set; }
	public string TargetUrl { get; set; }
	public int Position { get; set; }
	public DateTime StartDate { get; set; }
	public DateTime EndDate { get; set; }
	public bool IsActive { get; set; }
	public int DisplayOrder { get; set; }
	public int ClickCount { get; set; }
	public int ImpressionCount { get; set; }
	public DateTime CreatedAt { get; set; }
}
