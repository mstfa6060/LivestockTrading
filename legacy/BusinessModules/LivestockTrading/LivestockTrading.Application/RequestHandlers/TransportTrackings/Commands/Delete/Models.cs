namespace LivestockTrading.Application.RequestHandlers.TransportTrackings.Commands.Delete;

public class RequestModel : IRequestModel
{
	public Guid Id { get; set; }
}

public class ResponseModel : IResponseModel
{
	public bool Success { get; set; }
}
