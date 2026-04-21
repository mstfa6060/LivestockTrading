namespace LivestockTrading.Application.RequestHandlers.SellerSubscriptions.Queries.Detail;

public class RequestModel : IRequestModel
{
	public Guid SellerId { get; set; }
}

public class ResponseModel : IResponseModel
{
	public Guid Id { get; set; }
	public Guid SellerId { get; set; }
	public Guid SubscriptionPlanId { get; set; }
	public string PlanName { get; set; }
	public int PlanTier { get; set; }
	public int Status { get; set; }
	public int Period { get; set; }
	public int Platform { get; set; }
	public DateTime StartedAt { get; set; }
	public DateTime ExpiresAt { get; set; }
	public bool AutoRenew { get; set; }
	public int MaxActiveListings { get; set; }
	public int MaxPhotosPerListing { get; set; }
	public int MonthlyBoostCredits { get; set; }
	public bool HasDetailedAnalytics { get; set; }
	public int CurrentActiveListings { get; set; }
	public int RemainingListings { get; set; }
	public DateTime CreatedAt { get; set; }
}
