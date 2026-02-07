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
		return await _dbContext.Messages
			.AsNoTracking()
			.Where(m => m.RecipientUserId == userId && !m.IsRead && !m.IsDeleted)
			.GroupBy(m => m.ConversationId)
			.Select(g => new ConversationUnreadItem
			{
				ConversationId = g.Key,
				UnreadCount = g.Count(),
				LastMessageAt = g.Max(m => m.SentAt)
			})
			.OrderByDescending(x => x.LastMessageAt)
			.ToListAsync(ct);
	}
}
