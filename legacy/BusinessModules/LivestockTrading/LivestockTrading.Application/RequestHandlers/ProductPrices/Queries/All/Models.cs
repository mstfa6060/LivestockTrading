namespace LivestockTrading.Application.RequestHandlers.ProductPrices.Queries.All;

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
	public string CurrencyCode { get; set; }
	public double Price { get; set; }
	public double? DiscountedPrice { get; set; }
	public string CountryCodes { get; set; }
	public bool IsActive { get; set; }
	public DateTime? ValidFrom { get; set; }
	public DateTime? ValidUntil { get; set; }
	public bool IsAutomaticConversion { get; set; }
	public DateTime CreatedAt { get; set; }
}
