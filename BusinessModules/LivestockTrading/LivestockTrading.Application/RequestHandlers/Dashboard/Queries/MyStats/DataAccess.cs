namespace LivestockTrading.Application.RequestHandlers.Dashboard.Queries.MyStats;

public class DataAccess : IDataAccess
{
	private readonly LivestockTradingModuleDbContext _dbContext;

	public DataAccess(ArfBlocksDependencyProvider dbContextProvider)
	{
		_dbContext = dbContextProvider.GetInstance<LivestockTradingModuleDbContext>();
	}

	public async Task<Seller> GetSellerByUserId(Guid userId, CancellationToken ct)
	{
		return await _dbContext.Sellers
			.AsNoTracking()
			.FirstOrDefaultAsync(s => s.UserId == userId && !s.IsDeleted, ct);
	}

	public async Task<DashboardStats> GetStats(Guid sellerId, Guid userId, string period, CancellationToken ct)
	{
		// Listing counts are always for all time
		var products = _dbContext.Products.Where(p => p.SellerId == sellerId && !p.IsDeleted);

		var totalListings = await products.CountAsync(ct);
		var activeListings = await products.CountAsync(p => p.Status == ProductStatus.Active, ct);
		var draftListings = await products.CountAsync(p => p.Status == ProductStatus.Draft, ct);
		var pendingListings = await products.CountAsync(p => p.Status == ProductStatus.PendingApproval, ct);
		var soldListings = await products.CountAsync(p => p.Status == ProductStatus.Sold, ct);

		// Period filter for view/favorite/message counts
		var periodStart = GetPeriodStart(period);

		var totalViews = await products.SumAsync(p => p.ViewCount, ct);
		var totalFavorites = await products.SumAsync(p => p.FavoriteCount, ct);

		var messagesQuery = _dbContext.Messages
			.Where(m => (m.SenderUserId == userId || m.RecipientUserId == userId) && !m.IsDeleted);

		if (periodStart.HasValue)
			messagesQuery = messagesQuery.Where(m => m.SentAt >= periodStart.Value);

		var totalMessages = await messagesQuery.CountAsync(ct);

		var unreadMessages = await _dbContext.Messages
			.CountAsync(m => m.RecipientUserId == userId && !m.IsRead && !m.IsDeleted, ct);

		// Reviews
		var productIds = await products.Select(p => p.Id).ToListAsync(ct);
		var reviewsQuery = _dbContext.ProductReviews
			.Where(r => productIds.Contains(r.ProductId) && !r.IsDeleted);

		var totalReviews = await reviewsQuery.CountAsync(ct);
		var averageRating = totalReviews > 0
			? await reviewsQuery.AverageAsync(r => (decimal)r.Rating, ct)
			: 0m;

		return new DashboardStats
		{
			TotalListings = totalListings,
			ActiveListings = activeListings,
			DraftListings = draftListings,
			PendingListings = pendingListings,
			SoldListings = soldListings,
			TotalViews = totalViews,
			TotalFavorites = totalFavorites,
			TotalMessages = totalMessages,
			UnreadMessages = unreadMessages,
			TotalReviews = totalReviews,
			AverageRating = Math.Round(averageRating, 2)
		};
	}

	public async Task<List<ActivityItem>> GetRecentActivity(Guid sellerId, Guid userId, string period, CancellationToken ct)
	{
		var periodStart = GetPeriodStart(period);
		var activities = new List<ActivityItem>();

		// Recent messages
		var messagesQuery = _dbContext.Messages
			.AsNoTracking()
			.Where(m => m.RecipientUserId == userId && !m.IsDeleted);

		if (periodStart.HasValue)
			messagesQuery = messagesQuery.Where(m => m.SentAt >= periodStart.Value);

		var recentMessages = await messagesQuery
			.OrderByDescending(m => m.SentAt)
			.Take(5)
			.ToListAsync(ct);

		foreach (var msg in recentMessages)
		{
			activities.Add(new ActivityItem
			{
				Type = "new_message",
				EntityId = msg.ConversationId,
				EntityTitle = msg.Content != null && msg.Content.Length > 50
					? msg.Content.Substring(0, 50) + "..."
					: msg.Content,
				ActorName = null,
				CreatedAt = msg.SentAt
			});
		}

		// Recent favorites on seller's products
		var sellerProductIds = await _dbContext.Products
			.Where(p => p.SellerId == sellerId && !p.IsDeleted)
			.Select(p => p.Id)
			.ToListAsync(ct);

		var favoritesQuery = _dbContext.FavoriteProducts
			.AsNoTracking()
			.Include(f => f.Product)
			.Where(f => sellerProductIds.Contains(f.ProductId) && !f.IsDeleted);

		if (periodStart.HasValue)
			favoritesQuery = favoritesQuery.Where(f => f.CreatedAt >= periodStart.Value);

		var recentFavorites = await favoritesQuery
			.OrderByDescending(f => f.CreatedAt)
			.Take(5)
			.ToListAsync(ct);

		foreach (var fav in recentFavorites)
		{
			activities.Add(new ActivityItem
			{
				Type = "new_favorite",
				EntityId = fav.ProductId,
				EntityTitle = fav.Product?.Title,
				ActorName = null,
				CreatedAt = fav.CreatedAt
			});
		}

		// Recent reviews
		var reviewsQuery = _dbContext.ProductReviews
			.AsNoTracking()
			.Include(r => r.Product)
			.Where(r => sellerProductIds.Contains(r.ProductId) && !r.IsDeleted);

		if (periodStart.HasValue)
			reviewsQuery = reviewsQuery.Where(r => r.CreatedAt >= periodStart.Value);

		var recentReviews = await reviewsQuery
			.OrderByDescending(r => r.CreatedAt)
			.Take(5)
			.ToListAsync(ct);

		foreach (var review in recentReviews)
		{
			activities.Add(new ActivityItem
			{
				Type = "new_review",
				EntityId = review.ProductId,
				EntityTitle = review.Product?.Title,
				ActorName = null,
				CreatedAt = review.CreatedAt
			});
		}

		return activities.OrderByDescending(a => a.CreatedAt).Take(10).ToList();
	}

	private static DateTime? GetPeriodStart(string period)
	{
		return period?.ToLower() switch
		{
			"week" => DateTime.UtcNow.AddDays(-7),
			"month" => DateTime.UtcNow.AddMonths(-1),
			"year" => DateTime.UtcNow.AddYears(-1),
			_ => null // "all" or null
		};
	}
}
