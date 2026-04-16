namespace LivestockTrading.Application.RequestHandlers.AppVersions.Commands.Update;

public class RequestModel : IRequestModel
{
	public Guid Id { get; set; }
	public int Platform { get; set; }
	public string MinSupportedVersion { get; set; }
	public string LatestVersion { get; set; }
	public string StoreUrl { get; set; }
	public string UpdateMessage { get; set; }
	public bool IsActive { get; set; }
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
	public DateTime? UpdatedAt { get; set; }
}
