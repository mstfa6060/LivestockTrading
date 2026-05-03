using LivestockTrading.Domain.Entities;
using LivestockTrading.Infrastructure.RelationalDB;
using Microsoft.EntityFrameworkCore;

namespace LivestockTrading.Application.RequestHandlers.Conversations.Queries.All;

public class DataAccess : IDataAccess
{
	private readonly LivestockTradingModuleDbContext _dbContext;

	public DataAccess(ArfBlocksDependencyProvider dbContextProvider)
	{
		_dbContext = dbContextProvider.GetInstance<LivestockTradingModuleDbContext>();
	}

	public async Task<(List<Conversation> Conversations, Dictionary<Guid, ConversationExtras> Extras, XPageResponse Page)> All(
		Guid currentUserId,
		XSorting sorting,
		List<XFilterItem> filters,
		XPageRequest pageRequest,
		CancellationToken ct)
	{
		var query = _dbContext.Conversations
			.AsNoTracking()
			.Where(c => !c.IsDeleted)
			.Where(c => c.ParticipantUserId1 == currentUserId || c.ParticipantUserId2 == currentUserId)
			.Sort(sorting)
			.Filter(filters);

		if (sorting == null)
			query = query.OrderByDescending(c => c.LastMessageAt);

		var page = query.GetPage(pageRequest);
		var conversations = await query.Paginate(page).ToListAsync(ct);

		var ids = conversations.Select(c => c.Id).ToList();

		// Single round-trip: last message snippet + unread count per conversation, scoped to current user
		var extras = await _dbContext.Messages
			.AsNoTracking()
			.Where(m => !m.IsDeleted && ids.Contains(m.ConversationId))
			.GroupBy(m => m.ConversationId)
			.Select(g => new
			{
				ConversationId = g.Key,
				LastMessageContent = g.OrderByDescending(m => m.SentAt).Select(m => m.Content).FirstOrDefault(),
				LastMessageSenderUserId = (Guid?)g.OrderByDescending(m => m.SentAt).Select(m => m.SenderUserId).FirstOrDefault(),
				UnreadCount = g.Count(m => !m.IsRead && m.RecipientUserId == currentUserId)
			})
			.ToDictionaryAsync(
				x => x.ConversationId,
				x => new ConversationExtras
				{
					LastMessageContent = x.LastMessageContent,
					LastMessageSenderUserId = x.LastMessageSenderUserId,
					UnreadCount = x.UnreadCount
				},
				ct);

		return (conversations, extras, page);
	}
}

public class ConversationExtras
{
	public string LastMessageContent { get; set; }
	public Guid? LastMessageSenderUserId { get; set; }
	public int UnreadCount { get; set; }
}
