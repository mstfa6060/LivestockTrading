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

	public async Task<(List<Conversation> Conversations, XPageResponse Page)> All(
		XSorting sorting,
		List<XFilterItem> filters,
		XPageRequest pageRequest,
		CancellationToken ct)
	{
		var query = _dbContext.Conversations
			.AsNoTracking()
			.Where(c => !c.IsDeleted)
			.Sort(sorting)
			.Filter(filters);

		// Default sorting
		if (sorting == null)
			query = query.OrderByDescending(c => c.CreatedAt);

		// Pagination
		var page = query.GetPage(pageRequest);
		var conversations = await query.Paginate(page).ToListAsync(ct);

		return (conversations, page);
	}
}
