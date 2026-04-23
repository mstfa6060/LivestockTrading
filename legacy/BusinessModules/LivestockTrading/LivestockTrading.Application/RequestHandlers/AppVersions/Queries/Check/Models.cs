namespace LivestockTrading.Application.RequestHandlers.AppVersions.Queries.Check;

public class RequestModel : IRequestModel
{
	/// <summary>Platform: 0=Web, 1=Android, 2=iOS</summary>
	public int Platform { get; set; }
	/// <summary>Istemcinin calistirdigi sürüm ("1.2.3")</summary>
	public string CurrentVersion { get; set; }
}

public class ResponseModel : IResponseModel
{
	public string MinSupportedVersion { get; set; }
	public string LatestVersion { get; set; }
	public string StoreUrl { get; set; }
	/// <summary>0=OK (guncelleme yok), 1=SoftUpdate (onerilir), 2=ForceUpdate (zorunlu)</summary>
	public int UpdateType { get; set; }
	public string UpdateMessage { get; set; }
}
