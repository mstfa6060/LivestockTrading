namespace BaseModules.IAM.Application.RequestHandlers.Users.Commands.Delete;

public class RequestModel : IRequestModel
{
	/// <summary>Password confirmation for security</summary>
	public string Password { get; set; }

	/// <summary>Reason for deletion (optional)</summary>
	public string Reason { get; set; }
}

public class ResponseModel : IResponseModel
{
	public bool Success { get; set; }
	public string Message { get; set; }
}
