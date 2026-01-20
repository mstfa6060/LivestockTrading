namespace BaseModules.IAM.Application.RequestHandlers.Users.Commands.ForgotPassword;

public class RequestModel : IRequestModel
{
	public string Email { get; set; }
}

public class ResponseModel : IResponseModel
{
	public Guid UserId { get; set; }
	public string Email { get; set; }
	public DateTime? TokenExpiry { get; set; }
}

