namespace LivestockTrading.Application.RequestHandlers.Products.Queries.Detail;

public class RequestModel : IRequestModel
{
	public Guid Id { get; set; }
}

public class ResponseModel : IResponseModel
{
	public Guid Id { get; set; }
	public string Title { get; set; }
	public string Slug { get; set; }
	public string Description { get; set; }
	public string ShortDescription { get; set; }
	public Guid CategoryId { get; set; }
	public string CategoryName { get; set; }
	public Guid? BrandId { get; set; }
	public string BrandName { get; set; }
	public decimal BasePrice { get; set; }
	public string Currency { get; set; }
	public decimal? DiscountedPrice { get; set; }
	public string PriceUnit { get; set; }
	public int StockQuantity { get; set; }
	public string StockUnit { get; set; }
	public int? MinOrderQuantity { get; set; }
	public int? MaxOrderQuantity { get; set; }
	public bool IsInStock { get; set; }
	public Guid SellerId { get; set; }
	public string SellerName { get; set; }
	public Guid LocationId { get; set; }
	public int Status { get; set; }
	public int Condition { get; set; }
	public bool IsShippingAvailable { get; set; }
	public decimal? ShippingCost { get; set; }
	public bool IsInternationalShipping { get; set; }
	public decimal? Weight { get; set; }
	public string WeightUnit { get; set; }
	public string Attributes { get; set; }
	public string MetaTitle { get; set; }
	public string MetaDescription { get; set; }
	public string MetaKeywords { get; set; }
	public int ViewCount { get; set; }
	public int FavoriteCount { get; set; }
	public decimal? AverageRating { get; set; }
	public int ReviewCount { get; set; }
	public DateTime? PublishedAt { get; set; }
	public DateTime? ExpiresAt { get; set; }
	public DateTime CreatedAt { get; set; }
	public DateTime? UpdatedAt { get; set; }
}
