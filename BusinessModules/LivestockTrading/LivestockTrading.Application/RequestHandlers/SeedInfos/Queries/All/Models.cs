namespace LivestockTrading.Application.RequestHandlers.SeedInfos.Queries.All;

public class RequestModel : IRequestModel
{
	public XSorting Sorting { get; set; }
	public List<XFilterItem> Filters { get; set; }
	public XPageRequest PageRequest { get; set; }
}

public class ResponseModel : IResponseModel<Array>
{
	public Guid Id { get; set; }
	public Guid ProductId { get; set; }
	public int Type { get; set; }
	public string Variety { get; set; }
	public string ScientificName { get; set; }
	public double? GerminationRate { get; set; }
	public int? DaysToMaturity { get; set; }
	public bool IsOrganic { get; set; }
	public bool IsHybrid { get; set; }
	public DateTime CreatedAt { get; set; }
}
