namespace LivestockTrading.Application.RequestHandlers.Sellers.Commands.Suspend;

public class RequestModel : IRequestModel
{
	public Guid Id { get; set; }
	public string Reason { get; set; }
}

public class ResponseModel : IResponseModel
{
	public bool Success { get; set; }
	public Guid SellerId { get; set; }
	public DateTime SuspendedAt { get; set; }
}
