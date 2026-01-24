namespace LivestockTrading.Application.RequestHandlers.Deals.Queries.Detail;

public class RequestModel : IRequestModel
{
	public Guid Id { get; set; }
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
	public bool TransportRequestCreated { get; set; }
	public Guid? TransportRequestId { get; set; }
	public bool IsCompleted { get; set; }
	public DateTime? CompletedAt { get; set; }
	public bool IsCancelled { get; set; }
	public DateTime? CancelledAt { get; set; }
	public string CancellationReason { get; set; }
	public DateTime CreatedAt { get; set; }
	public DateTime? UpdatedAt { get; set; }
}
