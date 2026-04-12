namespace BaseModules.IAM.Application.RequestHandlers.Auth.Commands.VerifyEmailOtp;

public class RequestModel : IRequestModel
{
    public Guid UserId { get; set; }
    public string Email { get; set; }
    public string OtpCode { get; set; }
}

public class ResponseModel : IResponseModel
{
    public bool IsSuccess { get; set; }
}
