namespace BaseModules.IAM.Application.RequestHandlers.Auth.Commands.SendEmailOtp;

public class RequestModel : IRequestModel
{
    public Guid UserId { get; set; }
    public string Email { get; set; }
    public string Language { get; set; }
}

public class ResponseModel : IResponseModel
{
    public bool IsSuccess { get; set; }
}
