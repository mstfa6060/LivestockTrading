namespace LivestockTrading.Application.RequestHandlers.Currencies.Commands.Delete;

public class RequestModel : IRequestModel
{
	public Guid Id { get; set; }
}

public class ResponseModel : IResponseModel
{
	public bool Success { get; set; }
}
