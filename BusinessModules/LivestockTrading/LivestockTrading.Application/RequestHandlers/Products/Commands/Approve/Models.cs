namespace LivestockTrading.Application.RequestHandlers.Products.Commands.Approve;

public class RequestModel : IRequestModel
{
	public Guid Id { get; set; }
}

public class ResponseModel : IResponseModel
{
	public bool Success { get; set; }
	public Guid ProductId { get; set; }
	public int NewStatus { get; set; }
	public DateTime ApprovedAt { get; set; }
}
