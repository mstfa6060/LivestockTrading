namespace LivestockTrading.Application.RequestHandlers.AppVersions.Queries.All;

public class RequestModel : IRequestModel
{
	/// <summary>Opsiyonel platform filtresi: 0=Web, 1=Android, 2=iOS. null ise tum platformlar.</summary>
	public int? Platform { get; set; }
	public XSorting Sorting { get; set; }
	public List<XFilterItem> Filters { get; set; }
	public XPageRequest PageRequest { get; set; }
}

public class ResponseModel : IResponseModel<Array>
{
	public Guid Id { get; set; }
	public int Platform { get; set; }
	public string MinSupportedVersion { get; set; }
	public string LatestVersion { get; set; }
	public string StoreUrl { get; set; }
	public string UpdateMessage { get; set; }
	public bool IsActive { get; set; }
	public DateTime CreatedAt { get; set; }
	public DateTime? UpdatedAt { get; set; }
}
