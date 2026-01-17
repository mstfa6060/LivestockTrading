namespace BaseModules.Notification.Application.RequestHandlers.Push.Commands.RegisterToken;

public class RequestModel : IRequestModel
{
    public Guid UserId { get; set; }
    public string PushToken { get; set; }
    public string DeviceId { get; set; }
    public string AppName { get; set; }
    public UserPushPlatform Platform { get; set; }
}

public class ResponseModel : IResponseModel
{
    public bool Success { get; set; }
}
