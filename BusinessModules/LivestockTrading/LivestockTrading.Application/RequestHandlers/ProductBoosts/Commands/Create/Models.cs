namespace LivestockTrading.Application.RequestHandlers.ProductBoosts.Commands.Create;

public class RequestModel : IRequestModel
{
	public Guid ProductId { get; set; }
	public Guid SellerId { get; set; }
	public Guid BoostPackageId { get; set; }
	public int Platform { get; set; }
	public string Receipt { get; set; }
	public string StoreTransactionId { get; set; }
}

public class ResponseModel : IResponseModel
{
	public Guid Id { get; set; }
	public Guid ProductId { get; set; }
	public Guid SellerId { get; set; }
	public Guid BoostPackageId { get; set; }
	public DateTime StartedAt { get; set; }
	public DateTime ExpiresAt { get; set; }
	public int BoostScore { get; set; }
	public bool IsActive { get; set; }
	public DateTime CreatedAt { get; set; }
}
