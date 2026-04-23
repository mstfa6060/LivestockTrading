namespace LivestockTrading.Application.RequestHandlers.TransportRequests.Queries.All;

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
	public Guid SellerId { get; set; }
	public Guid BuyerId { get; set; }
	public Guid PickupLocationId { get; set; }
	public Guid DeliveryLocationId { get; set; }
	public int TransportType { get; set; }
	public int Status { get; set; }
	public bool IsUrgent { get; set; }
	public bool IsInPool { get; set; }
	public Guid? AssignedTransporterId { get; set; }
	public DateTime CreatedAt { get; set; }
}
