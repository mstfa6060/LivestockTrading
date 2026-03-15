using LivestockTrading.Domain.Entities;
using LivestockTrading.Infrastructure.RelationalDB;
using Microsoft.EntityFrameworkCore;

namespace LivestockTrading.Application.RequestHandlers.SubscriptionPlans.Queries.All;

public class DataAccess : IDataAccess
{
	private readonly LivestockTradingModuleDbContext _dbContext;

	public DataAccess(ArfBlocksDependencyProvider dbContextProvider)
	{
		_dbContext = dbContextProvider.GetInstance<LivestockTradingModuleDbContext>();
	}

	public async Task<(List<SubscriptionPlan> Plans, XPageResponse Page)> All(
		string languageCode,
		int? targetType,
		XSorting sorting,
		List<XFilterItem> filters,
		XPageRequest pageRequest,
		CancellationToken ct)
	{
		var query = _dbContext.SubscriptionPlans
			.AsNoTracking()
			.Where(p => !p.IsDeleted && p.IsActive)
			.Sort(sorting)
			.Filter(filters);

		if (targetType.HasValue)
			query = query.Where(p => (int)p.TargetType == targetType.Value);

		if (sorting == null)
			query = query.OrderBy(p => p.SortOrder);

		var page = query.GetPage(pageRequest);
		var plans = await query.Paginate(page).ToListAsync(ct);

		return (plans, page);
	}
}
