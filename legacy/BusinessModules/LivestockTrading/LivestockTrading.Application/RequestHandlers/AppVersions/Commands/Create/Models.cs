namespace LivestockTrading.Application.RequestHandlers.AppVersions.Commands.Create;

public class RequestModel : IRequestModel
{
	/// <summary>Platform: 0=Web, 1=Android, 2=iOS</summary>
	public int Platform { get; set; }
	public string MinSupportedVersion { get; set; }
	public string LatestVersion { get; set; }
	public string StoreUrl { get; set; }
	public string UpdateMessage { get; set; }
	public bool IsActive { get; set; } = true;
}

public class ResponseModel : IResponseModel
{
	public Guid Id { get; set; }
	public int Platform { get; set; }
	public string MinSupportedVersion { get; set; }
	public string LatestVersion { get; set; }
	public string StoreUrl { get; set; }
	public string UpdateMessage { get; set; }
	public bool IsActive { get; set; }
	public DateTime CreatedAt { get; set; }
}
