using LivestockTrading.Domain.Entities;

namespace LivestockTrading.Application.RequestHandlers.Dashboard.Queries.Stats;

public class Mapper
{
	public ResponseModel MapToResponse(
		Seller seller,
		int totalListings,
		int activeListings,
		int totalViews,
		int totalFavorites,
		int totalMessages,
		List<Product> recentProducts,
		List<Message> recentMessages)
	{
		var recentActivity = new List<RecentActivityItem>();

		foreach (var product in recentProducts)
		{
			recentActivity.Add(new RecentActivityItem
			{
				Type = "product",
				Title = product.Title,
				Description = $"Urun durumu: {product.Status}",
				Date = product.CreatedAt,
				ReferenceId = product.Id
			});
		}

		foreach (var message in recentMessages)
		{
			recentActivity.Add(new RecentActivityItem
			{
				Type = "message",
				Title = "Yeni mesaj",
				Description = message.Content?.Length > 50
					? message.Content.Substring(0, 50) + "..."
					: message.Content,
				Date = message.SentAt,
				ReferenceId = message.ConversationId
			});
		}

		recentActivity = recentActivity.OrderByDescending(a => a.Date).Take(10).ToList();

		return new ResponseModel
		{
			TotalListings = totalListings,
			ActiveListings = activeListings,
			TotalViews = totalViews,
			TotalFavorites = totalFavorites,
			TotalMessages = totalMessages,
			TotalSales = seller.TotalSales,
			Revenue = seller.TotalRevenue,
			RecentActivity = recentActivity
		};
	}
}
