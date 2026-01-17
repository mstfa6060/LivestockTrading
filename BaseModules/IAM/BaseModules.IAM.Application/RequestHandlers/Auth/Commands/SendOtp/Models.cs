namespace BaseModules.IAM.Application.RequestHandlers.Auth.Commands.SendOtp;

public class RequestModel : IRequestModel
{
    public Guid UserId { get; set; }
    public string PhoneNumber { get; set; }
    public Guid CompanyId { get; set; }
    public string Language { get; set; }
}



public class ResponseModel : IResponseModel
{
    public bool IsSuccess { get; set; }
    public string OtpCode { get; set; }
}


