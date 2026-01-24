namespace LivestockTrading.Application.RequestHandlers.Currencies.Queries.All;

public class RequestModel : IRequestModel
{
	public XSorting Sorting { get; set; }
	public List<XFilterItem> Filters { get; set; }
	public XPageRequest PageRequest { get; set; }
}

public class ResponseModel : IResponseModel<Array>
{
	public Guid Id { get; set; }
	public string Code { get; set; }
	public string Symbol { get; set; }
	public string Name { get; set; }
	public decimal ExchangeRateToUSD { get; set; }
	public DateTime LastUpdated { get; set; }
	public bool IsActive { get; set; }
	public DateTime CreatedAt { get; set; }
}
