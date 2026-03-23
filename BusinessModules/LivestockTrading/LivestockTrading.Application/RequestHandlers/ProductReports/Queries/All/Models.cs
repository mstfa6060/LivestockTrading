namespace LivestockTrading.Application.RequestHandlers.ProductReports.Queries.All;

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
	public Guid ReporterUserId { get; set; }
	public int Reason { get; set; }
	public string Description { get; set; }
	public int Status { get; set; }
	public string AdminNote { get; set; }
	public Guid? ReviewedByUserId { get; set; }
	public DateTime? ReviewedAt { get; set; }
	public DateTime CreatedAt { get; set; }
}
