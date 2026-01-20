
namespace BaseModules.Notification.Application.RequestHandlers.MobilApplicationVersiyon.Queries.GetVersion;

public class RequestModel : IRequestModel
{
    public string Platform { get; set; }
}

public class ResponseModel : IResponseModel
{
    public string MinVersion { get; set; }
    public string LatestVersion { get; set; }
    public bool ForceUpdate { get; set; }
    public string UpdateMessage { get; set; }
    public string StoreUrl { get; set; }
}
