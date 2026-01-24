namespace LivestockTrading.Application.RequestHandlers.UserPreferences.Queries.All;

public class RequestModel : IRequestModel
{
	public XSorting Sorting { get; set; }
	public List<XFilterItem> Filters { get; set; }
	public XPageRequest PageRequest { get; set; }
}

public class ResponseModel : IResponseModel<Array>
{
	public Guid Id { get; set; }
	public Guid UserId { get; set; }
	public string PreferredCurrency { get; set; }
	public string PreferredLanguage { get; set; }
	public string CountryCode { get; set; }
	public string TimeZone { get; set; }
	public int WeightSystem { get; set; }
	public int DistanceSystem { get; set; }
	public int AreaSystem { get; set; }
	public bool EmailNotificationsEnabled { get; set; }
	public bool SmsNotificationsEnabled { get; set; }
	public bool PushNotificationsEnabled { get; set; }
	public bool DarkModeEnabled { get; set; }
	public int ProductsPerPage { get; set; }
	public int DefaultViewMode { get; set; }
	public DateTime CreatedAt { get; set; }
}
