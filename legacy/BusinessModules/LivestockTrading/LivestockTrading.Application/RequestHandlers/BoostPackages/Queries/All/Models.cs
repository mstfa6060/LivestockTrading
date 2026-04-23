namespace LivestockTrading.Application.RequestHandlers.BoostPackages.Queries.All;

public class RequestModel : IRequestModel
{
	public string LanguageCode { get; set; }
	public XSorting Sorting { get; set; }
	public List<XFilterItem> Filters { get; set; }
	public XPageRequest PageRequest { get; set; }
}

public class ResponseModel : IResponseModel<Array>
{
	public Guid Id { get; set; }
	public string Name { get; set; }
	public string Description { get; set; }
	public int DurationHours { get; set; }
	public double Price { get; set; }
	public string Currency { get; set; }
	public string AppleProductId { get; set; }
	public string GoogleProductId { get; set; }
	public int BoostType { get; set; }
	public int BoostScore { get; set; }
	public int SortOrder { get; set; }
	public bool IsActive { get; set; }
	public DateTime CreatedAt { get; set; }
}
