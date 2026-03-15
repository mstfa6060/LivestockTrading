namespace LivestockTrading.Application.RequestHandlers.SellerSubscriptions.Commands.Update;

public class RequestModel : IRequestModel
{
	public Guid Id { get; set; }
	public int? Status { get; set; }
	public bool? AutoRenew { get; set; }
	public string Receipt { get; set; }
}

public class ResponseModel : IResponseModel
{
	public Guid Id { get; set; }
	public Guid SellerId { get; set; }
	public Guid SubscriptionPlanId { get; set; }
	public int Status { get; set; }
	public int Period { get; set; }
	public int Platform { get; set; }
	public DateTime StartedAt { get; set; }
	public DateTime ExpiresAt { get; set; }
	public bool AutoRenew { get; set; }
	public DateTime? CancelledAt { get; set; }
	public DateTime CreatedAt { get; set; }
	public DateTime? UpdatedAt { get; set; }
}
