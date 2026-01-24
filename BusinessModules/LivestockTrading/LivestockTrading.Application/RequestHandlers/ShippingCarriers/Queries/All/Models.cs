namespace LivestockTrading.Application.RequestHandlers.ShippingCarriers.Queries.All;

public class RequestModel : IRequestModel
{
	public XSorting Sorting { get; set; }
	public List<XFilterItem> Filters { get; set; }
	public XPageRequest PageRequest { get; set; }
}

public class ResponseModel : IResponseModel<Array>
{
	public Guid Id { get; set; }
	public string Name { get; set; }
	public string Code { get; set; }
	public string Website { get; set; }
	public string TrackingUrlTemplate { get; set; }
	public bool IsActive { get; set; }
	public string SupportedCountries { get; set; }
	public DateTime CreatedAt { get; set; }
}
