namespace LivestockTrading.Application.RequestHandlers.Deals.Commands.Create;

public class RequestModel : IRequestModel
{
	public string DealNumber { get; set; }
	public Guid ProductId { get; set; }
	public Guid SellerId { get; set; }
	public Guid BuyerId { get; set; }
	public decimal AgreedPrice { get; set; }
	public string Currency { get; set; }
	public int Quantity { get; set; }
	public int Status { get; set; }
	public DateTime DealDate { get; set; }
	public int DeliveryMethod { get; set; }
	public DateTime? DeliveryDate { get; set; }
	public string BuyerNotes { get; set; }
	public string SellerNotes { get; set; }
	public string ContractDocuments { get; set; }
}

public class ResponseModel : IResponseModel
{
	public Guid Id { get; set; }
	public string DealNumber { get; set; }
	public Guid ProductId { get; set; }
	public Guid SellerId { get; set; }
	public Guid BuyerId { get; set; }
	public decimal AgreedPrice { get; set; }
	public string Currency { get; set; }
	public int Quantity { get; set; }
	public int Status { get; set; }
	public DateTime DealDate { get; set; }
	public int DeliveryMethod { get; set; }
	public DateTime? DeliveryDate { get; set; }
	public string BuyerNotes { get; set; }
	public string SellerNotes { get; set; }
	public string ContractDocuments { get; set; }
	public DateTime CreatedAt { get; set; }
}
