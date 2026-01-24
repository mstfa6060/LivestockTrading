namespace LivestockTrading.Application.RequestHandlers.ProductPrices.Queries.Detail;

public class RequestModel : IRequestModel
{
	public Guid Id { get; set; }
}

public class ResponseModel : IResponseModel
{
	public Guid Id { get; set; }
	public Guid ProductId { get; set; }
	public string CurrencyCode { get; set; }
	public decimal Price { get; set; }
	public decimal? DiscountedPrice { get; set; }
	public string CountryCodes { get; set; }
	public bool IsActive { get; set; }
	public DateTime? ValidFrom { get; set; }
	public DateTime? ValidUntil { get; set; }
	public bool IsAutomaticConversion { get; set; }
	public DateTime CreatedAt { get; set; }
	public DateTime? UpdatedAt { get; set; }
}
