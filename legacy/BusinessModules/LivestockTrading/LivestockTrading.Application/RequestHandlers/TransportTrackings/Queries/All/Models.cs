namespace LivestockTrading.Application.RequestHandlers.TransportTrackings.Queries.All;

public class RequestModel : IRequestModel
{
	public XSorting Sorting { get; set; }
	public List<XFilterItem> Filters { get; set; }
	public XPageRequest PageRequest { get; set; }
}

public class ResponseModel : IResponseModel<Array>
{
	public Guid Id { get; set; }
	public Guid TransportRequestId { get; set; }
	public double? Latitude { get; set; }
	public double? Longitude { get; set; }
	public string LocationDescription { get; set; }
	public int Status { get; set; }
	public string StatusDescription { get; set; }
	public DateTime RecordedAt { get; set; }
	public string Notes { get; set; }
	public string PhotoUrls { get; set; }
	public DateTime CreatedAt { get; set; }
}
