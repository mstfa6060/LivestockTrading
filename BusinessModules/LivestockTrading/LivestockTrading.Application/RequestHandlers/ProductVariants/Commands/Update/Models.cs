namespace LivestockTrading.Application.RequestHandlers.ProductVariants.Commands.Update;

public class RequestModel : IRequestModel
{
	public Guid Id { get; set; }
	public Guid ProductId { get; set; }
	public string Name { get; set; }
	public string SKU { get; set; }
	public decimal? Price { get; set; }
	public decimal? DiscountedPrice { get; set; }
	public int StockQuantity { get; set; }
	public bool IsInStock { get; set; }
	public string Attributes { get; set; }
	public string ImageUrl { get; set; }
	public bool IsActive { get; set; }
	public int SortOrder { get; set; }
}

public class ResponseModel : IResponseModel
{
	public Guid Id { get; set; }
	public Guid ProductId { get; set; }
	public string Name { get; set; }
	public string SKU { get; set; }
	public decimal? Price { get; set; }
	public decimal? DiscountedPrice { get; set; }
	public int StockQuantity { get; set; }
	public bool IsInStock { get; set; }
	public string Attributes { get; set; }
	public string ImageUrl { get; set; }
	public bool IsActive { get; set; }
	public int SortOrder { get; set; }
	public DateTime? UpdatedAt { get; set; }
}
