namespace LivestockTrading.Application.RequestHandlers.Dashboard.Queries.MyStats;

public class RequestModel : IRequestModel
{
	/// <summary>
	/// Satıcının UserId'si. JWT'den alınabilir veya frontend'den gönderilebilir.
	/// </summary>
	public Guid UserId { get; set; }

	/// <summary>
	/// İstatistik dönemi: "week", "month", "year", "all" (varsayılan: "all")
	/// </summary>
	public string Period { get; set; }
}

public class ResponseModel : IResponseModel
{
	public int TotalListings { get; set; }
	public int ActiveListings { get; set; }
	public int DraftListings { get; set; }
	public int PendingListings { get; set; }
	public int SoldListings { get; set; }
	public int TotalViews { get; set; }
	public int TotalFavorites { get; set; }
	public int TotalMessages { get; set; }
	public int UnreadMessages { get; set; }
	public int TotalReviews { get; set; }
	public decimal AverageRating { get; set; }
	public List<ActivityItem> RecentActivity { get; set; }
}

public class ActivityItem
{
	public string Type { get; set; }
	public Guid EntityId { get; set; }
	public string EntityTitle { get; set; }
	public string ActorName { get; set; }
	public DateTime CreatedAt { get; set; }
}

public class DashboardStats
{
	public int TotalListings { get; set; }
	public int ActiveListings { get; set; }
	public int DraftListings { get; set; }
	public int PendingListings { get; set; }
	public int SoldListings { get; set; }
	public int TotalViews { get; set; }
	public int TotalFavorites { get; set; }
	public int TotalMessages { get; set; }
	public int UnreadMessages { get; set; }
	public int TotalReviews { get; set; }
	public decimal AverageRating { get; set; }
}
