namespace LivestockTrading.Application.RequestHandlers.Sellers.Commands.Suspend;

public class RequestModel : IRequestModel
{
	public Guid SellerId { get; set; }
	public string SuspensionReason { get; set; }
}

public class ResponseModel : IResponseModel
{
	public bool Success { get; set; }
	public Guid SellerId { get; set; }
	public DateTime SuspendedAt { get; set; }
}
