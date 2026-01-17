namespace BaseModules.IAM.Application.RequestHandlers.Users.Commands.UpdatePassword;

// Request Model
public class RequestModel : IRequestModel
{
	public Guid UserId { get; set; }
	public string OldPassword { get; set; }
	public string NewPassword { get; set; }
}

// Response Model
public class ResponseModel : IResponseModel
{
	public Guid Id { get; set; }
}