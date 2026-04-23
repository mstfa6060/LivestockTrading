namespace LivestockTrading.Application.RequestHandlers.ProductVariants.Queries.All;

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
	public string Name { get; set; }
	public string SKU { get; set; }
	public double? Price { get; set; }
	public double? DiscountedPrice { get; set; }
	public int StockQuantity { get; set; }
	public bool IsInStock { get; set; }
	public string Attributes { get; set; }
	public string ImageUrl { get; set; }
	public bool IsActive { get; set; }
	public int SortOrder { get; set; }
	public DateTime CreatedAt { get; set; }
}
