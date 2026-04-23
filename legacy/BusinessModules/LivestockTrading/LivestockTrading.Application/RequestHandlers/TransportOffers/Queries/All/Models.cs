namespace LivestockTrading.Application.RequestHandlers.TransportOffers.Queries.All;

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
	public Guid TransporterId { get; set; }
	public double OfferedPrice { get; set; }
	public string Currency { get; set; }
	public int Status { get; set; }
	public int? EstimatedDurationDays { get; set; }
	public bool InsuranceIncluded { get; set; }
	public DateTime OfferDate { get; set; }
	public DateTime CreatedAt { get; set; }
}
