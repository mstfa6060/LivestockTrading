using LivestockTrading.Domain.Entities;
using LivestockTrading.Infrastructure.RelationalDB;
using Microsoft.EntityFrameworkCore;

namespace LivestockTrading.Application.RequestHandlers.UserPreferences.Queries.All;

public class DataAccess : IDataAccess
{
	private readonly LivestockTradingModuleDbContext _dbContext;

	public DataAccess(ArfBlocksDependencyProvider dependencyProvider)
	{
		_dbContext = dependencyProvider.GetInstance<LivestockTradingModuleDbContext>();
	}

	public async Task<(List<Domain.Entities.UserPreferences> Items, XPageResponse Page)> All(
		XSorting sorting,
		List<XFilterItem> filters,
		XPageRequest pageRequest,
		CancellationToken ct)
	{
		var query = _dbContext.UserPreferences
			.AsNoTracking()
			.Where(e => !e.IsDeleted)
			.Sort(sorting)
			.Filter(filters);

		if (sorting == null)
			query = query.OrderByDescending(e => e.CreatedAt);

		var page = query.GetPage(pageRequest);
		var items = await query.Paginate(page).ToListAsync(ct);

		return (items, page);
	}
}
