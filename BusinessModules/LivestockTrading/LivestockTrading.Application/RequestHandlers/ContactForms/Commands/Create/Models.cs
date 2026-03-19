namespace LivestockTrading.Application.RequestHandlers.ContactForms.Commands.Create;

public class RequestModel : IRequestModel
{
	public string Name { get; set; }
	public string Email { get; set; }
	public string Subject { get; set; }
	public string Message { get; set; }
}

public class ResponseModel : IResponseModel
{
	public Guid Id { get; set; }
	public bool Success { get; set; }
}
