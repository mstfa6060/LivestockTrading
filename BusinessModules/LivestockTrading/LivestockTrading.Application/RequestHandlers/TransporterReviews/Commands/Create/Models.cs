namespace LivestockTrading.Application.RequestHandlers.TransporterReviews.Commands.Create;

public class RequestModel : IRequestModel
{
	public Guid TransporterId { get; set; }
	public Guid UserId { get; set; }
	public Guid TransportRequestId { get; set; }
	public int OverallRating { get; set; }
	public int TimelinessRating { get; set; }
	public int CommunicationRating { get; set; }
	public int CarefulHandlingRating { get; set; }
	public int ProfessionalismRating { get; set; }
	public string Comment { get; set; }
}

public class ResponseModel : IResponseModel
{
	public Guid Id { get; set; }
	public Guid TransporterId { get; set; }
	public Guid UserId { get; set; }
	public Guid TransportRequestId { get; set; }
	public int OverallRating { get; set; }
	public int TimelinessRating { get; set; }
	public int CommunicationRating { get; set; }
	public int CarefulHandlingRating { get; set; }
	public int ProfessionalismRating { get; set; }
	public string Comment { get; set; }
	public DateTime CreatedAt { get; set; }
}
