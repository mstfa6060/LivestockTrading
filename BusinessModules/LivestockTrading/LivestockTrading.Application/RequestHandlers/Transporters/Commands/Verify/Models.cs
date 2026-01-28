namespace LivestockTrading.Application.RequestHandlers.Transporters.Commands.Verify;

public class RequestModel : IRequestModel
{
	public Guid Id { get; set; }
}

public class ResponseModel : IResponseModel
{
	public bool Success { get; set; }
	public Guid TransporterId { get; set; }
	public DateTime VerifiedAt { get; set; }
}
