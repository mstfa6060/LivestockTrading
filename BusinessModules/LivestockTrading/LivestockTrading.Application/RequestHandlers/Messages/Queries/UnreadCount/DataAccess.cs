using LivestockTrading.Infrastructure.RelationalDB;

namespace LivestockTrading.Application.RequestHandlers.Messages.Queries.UnreadCount;

public class DataAccess : IDataAccess
{
	private readonly LivestockTradingModuleDbContext _dbContext;

	public DataAccess(ArfBlocksDependencyProvider dbContextProvider)
	{
		_dbContext = dbContextProvider.GetInstance<LivestockTradingModuleDbContext>();
	}

	public async Task<List<ConversationUnreadItem>> GetUnreadCounts(Guid userId, CancellationToken ct)
	{
		var unreadMessages = await _dbContext.Messages
			.AsNoTracking()
			.Where(m => m.RecipientUserId == userId && !m.IsRead && !m.IsDeleted)
			.GroupBy(m => m.ConversationId)
			.Select(g => new
			{
				ConversationId = g.Key,
				UnreadCount = g.Count(),
				LastMessageAt = g.Max(m => m.SentAt),
				LastMessageContent = g.OrderByDescending(m => m.SentAt).Select(m => m.Content).FirstOrDefault(),
				LastSenderUserId = g.OrderByDescending(m => m.SentAt).Select(m => m.SenderUserId).FirstOrDefault()
			})
			.OrderByDescending(x => x.LastMessageAt)
			.ToListAsync(ct);

		return unreadMessages.Select(u => new ConversationUnreadItem
		{
			ConversationId = u.ConversationId,
			UnreadCount = u.UnreadCount,
			LastMessage = u.LastMessageContent != null && u.LastMessageContent.Length > 100
				? u.LastMessageContent.Substring(0, 100)
				: u.LastMessageContent,
			LastMessageAt = u.LastMessageAt,
			SenderUserId = u.LastSenderUserId,
			SenderDisplayName = null // Resolved by frontend from user cache
		}).ToList();
	}
}
