namespace LivestockTrading.Application.RequestHandlers.Products.Commands.Reject;

public class RequestModel : IRequestModel
{
	public Guid Id { get; set; }
	public string Reason { get; set; }
}

public class ResponseModel : IResponseModel
{
	public bool Success { get; set; }
	public Guid ProductId { get; set; }
	public int NewStatus { get; set; }
	public string RejectionReason { get; set; }
	public DateTime RejectedAt { get; set; }
}
