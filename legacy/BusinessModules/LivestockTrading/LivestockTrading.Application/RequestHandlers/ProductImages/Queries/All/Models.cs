namespace LivestockTrading.Application.RequestHandlers.ProductImages.Queries.All;

public class RequestModel : IRequestModel
{
	public Guid ProductId { get; set; }
	public XSorting Sorting { get; set; }
	public List<XFilterItem> Filters { get; set; }
	public XPageRequest PageRequest { get; set; }
}

public class ResponseModel : IResponseModel<Array>
{
	public Guid Id { get; set; }
	public Guid ProductId { get; set; }
	public string ImageUrl { get; set; }
	public string ThumbnailUrl { get; set; }
	public string AltText { get; set; }
	public int SortOrder { get; set; }
	public bool IsPrimary { get; set; }
	public DateTime CreatedAt { get; set; }
}
