namespace LivestockTrading.Application.RequestHandlers.SubscriptionPlans.Queries.Detail;

public class RequestModel : IRequestModel
{
	public Guid Id { get; set; }
	public string LanguageCode { get; set; }
}

public class ResponseModel : IResponseModel
{
	public Guid Id { get; set; }
	public string Name { get; set; }
	public string Description { get; set; }
	public int TargetType { get; set; }
	public int Tier { get; set; }
	public double PriceMonthly { get; set; }
	public double PriceYearly { get; set; }
	public string Currency { get; set; }
	public string AppleProductIdMonthly { get; set; }
	public string AppleProductIdYearly { get; set; }
	public string GoogleProductIdMonthly { get; set; }
	public string GoogleProductIdYearly { get; set; }
	public int MaxActiveListings { get; set; }
	public int MaxPhotosPerListing { get; set; }
	public int MonthlyBoostCredits { get; set; }
	public bool HasDetailedAnalytics { get; set; }
	public bool HasPrioritySupport { get; set; }
	public bool HasFeaturedBadge { get; set; }
	public int SortOrder { get; set; }
	public bool IsActive { get; set; }
	public DateTime CreatedAt { get; set; }
	public DateTime? UpdatedAt { get; set; }
}
