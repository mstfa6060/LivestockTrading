namespace LivestockTrading.Application.RequestHandlers.TransporterReviews.Queries.All;

public class RequestModel : IRequestModel
{
	public XSorting Sorting { get; set; }
	public List<XFilterItem> Filters { get; set; }
	public XPageRequest PageRequest { get; set; }
}

public class ResponseModel : IResponseModel<Array>
{
	public Guid Id { get; set; }
	public Guid TransporterId { get; set; }
	public Guid UserId { get; set; }
	public int OverallRating { get; set; }
	public int TimelinessRating { get; set; }
	public int CommunicationRating { get; set; }
	public int CarefulHandlingRating { get; set; }
	public int ProfessionalismRating { get; set; }
	public bool IsApproved { get; set; }
	public DateTime CreatedAt { get; set; }
}
