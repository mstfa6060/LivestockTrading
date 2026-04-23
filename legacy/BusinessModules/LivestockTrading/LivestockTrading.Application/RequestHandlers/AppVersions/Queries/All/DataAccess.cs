using LivestockTrading.Domain.Entities;
using LivestockTrading.Infrastructure.RelationalDB;
using Microsoft.EntityFrameworkCore;

namespace LivestockTrading.Application.RequestHandlers.AppVersions.Queries.All;

public class DataAccess : IDataAccess
{
	private readonly LivestockTradingModuleDbContext _dbContext;

	public DataAccess(ArfBlocksDependencyProvider dbContextProvider)
	{
		_dbContext = dbContextProvider.GetInstance<LivestockTradingModuleDbContext>();
	}

	public async Task<(List<AppVersionConfig> Items, XPageResponse Page)> All(
		int? platform,
		XSorting sorting,
		List<XFilterItem> filters,
		XPageRequest pageRequest,
		CancellationToken ct)
	{
		var query = _dbContext.AppVersionConfigs
			.AsNoTracking()
			.Where(e => !e.IsDeleted);

		if (platform.HasValue)
			query = query.Where(e => e.Platform == platform.Value);

		query = query.Sort(sorting).Filter(filters);

		if (sorting == null)
			query = query.OrderByDescending(e => e.CreatedAt);

		var page = query.GetPage(pageRequest);
		var items = await query.Paginate(page).ToListAsync(ct);

		return (items, page);
	}
}
