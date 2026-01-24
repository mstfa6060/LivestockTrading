namespace LivestockTrading.Application.RequestHandlers.Offers.Commands.Create;

public class RequestModel : IRequestModel
{
	public Guid ProductId { get; set; }
	public Guid BuyerUserId { get; set; }
	public Guid SellerUserId { get; set; }
	public decimal OfferedPrice { get; set; }
	public string Currency { get; set; }
	public int Quantity { get; set; }
	public string Message { get; set; }
	public int Status { get; set; }
	public DateTime? ExpiryDate { get; set; }
	public Guid? CounterOfferToId { get; set; }
}

public class ResponseModel : IResponseModel
{
	public Guid Id { get; set; }
	public Guid ProductId { get; set; }
	public Guid BuyerUserId { get; set; }
	public Guid SellerUserId { get; set; }
	public decimal OfferedPrice { get; set; }
	public string Currency { get; set; }
	public int Quantity { get; set; }
	public string Message { get; set; }
	public int Status { get; set; }
	public DateTime OfferDate { get; set; }
	public DateTime? ExpiryDate { get; set; }
	public Guid? CounterOfferToId { get; set; }
	public DateTime CreatedAt { get; set; }
}
