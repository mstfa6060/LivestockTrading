namespace LivestockTrading.Application.RequestHandlers.ProductReports.Commands.Create;

public class RequestModel : IRequestModel
{
	public Guid ProductId { get; set; }
	public int Reason { get; set; }
	public string Description { get; set; }
}

public class ResponseModel : IResponseModel
{
	public Guid Id { get; set; }
	public bool Success { get; set; }
}
