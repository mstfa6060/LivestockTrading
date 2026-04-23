namespace LivestockTrading.Application.RequestHandlers.ProductBoosts.Queries.All;

public class RequestModel : IRequestModel
{
	public Guid SellerId { get; set; }
	public XSorting Sorting { get; set; }
	public List<XFilterItem> Filters { get; set; }
	public XPageRequest PageRequest { get; set; }
}

public class ResponseModel : IResponseModel<Array>
{
	public Guid Id { get; set; }
	public Guid ProductId { get; set; }
	public string ProductTitle { get; set; }
	public Guid BoostPackageId { get; set; }
	public string BoostPackageName { get; set; }
	public int BoostType { get; set; }
	public DateTime StartedAt { get; set; }
	public DateTime ExpiresAt { get; set; }
	public int BoostScore { get; set; }
	public bool IsActive { get; set; }
	public bool IsExpired { get; set; }
	public DateTime CreatedAt { get; set; }
}
