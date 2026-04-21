namespace LivestockTrading.Application.RequestHandlers.Dashboard.Queries.Stats;

public class RequestModel : IRequestModel
{
	/// <summary>
	/// Satıcının UserId'si. JWT'den alınabilir veya frontend'den gönderilebilir.
	/// </summary>
	public Guid UserId { get; set; }
}

public class ResponseModel : IResponseModel
{
	public int TotalListings { get; set; }
	public int ActiveListings { get; set; }
	public int TotalViews { get; set; }
	public int TotalFavorites { get; set; }
	public int TotalMessages { get; set; }
	public int TotalSales { get; set; }
	public double Revenue { get; set; }
	public List<RecentActivityItem> RecentActivity { get; set; }
}

public class RecentActivityItem
{
	public string Type { get; set; }
	public string Title { get; set; }
	public string Description { get; set; }
	public DateTime Date { get; set; }
	public Guid? ReferenceId { get; set; }
}
