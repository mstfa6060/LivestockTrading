namespace BaseModules.Notification.Application.RequestHandlers.Push.Commands.RevokeToken;

public class RequestModel : IRequestModel
{
    public Guid UserId { get; set; }
    public string DeviceId { get; set; }
}

public class ResponseModel : IResponseModel
{
    public bool Success { get; set; }
}
