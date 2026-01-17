namespace BaseModules.IAM.Application.RequestHandlers.Users.Commands.ResetPassword;

public class RequestModel : IRequestModel
{
	public string Token { get; set; }
	public string NewPassword { get; set; }
}

public class ResponseModel : IResponseModel
{
	public Guid Id { get; set; }
}
