namespace LivestockTrading.Application.RequestHandlers.Farms.Queries.All;

public class RequestModel : IRequestModel
{
	public XSorting Sorting { get; set; }
	public List<XFilterItem> Filters { get; set; }
	public XPageRequest PageRequest { get; set; }
}

public class ResponseModel : IResponseModel<Array>
{
	public Guid Id { get; set; }
	public string Name { get; set; }
	public Guid SellerId { get; set; }
	public Guid LocationId { get; set; }
	public int Type { get; set; }
	public decimal? TotalAreaHectares { get; set; }
	public bool IsOrganic { get; set; }
	public bool IsActive { get; set; }
	public bool IsVerified { get; set; }
	public DateTime CreatedAt { get; set; }
}
