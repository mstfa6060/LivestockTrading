namespace LivestockTrading.Application.RequestHandlers.Transporters.Commands.Suspend;

public class RequestModel : IRequestModel
{
	public Guid TransporterId { get; set; }
	public string SuspensionReason { get; set; }
}

public class ResponseModel : IResponseModel
{
	public bool Success { get; set; }
	public Guid TransporterId { get; set; }
	public DateTime SuspendedAt { get; set; }
}
