using LivestockTrading.Domain.Entities;
using LivestockTrading.Infrastructure.RelationalDB;
using Microsoft.EntityFrameworkCore;

namespace LivestockTrading.Application.RequestHandlers.BoostPackages.Queries.All;

public class DataAccess : IDataAccess
{
	private readonly LivestockTradingModuleDbContext _dbContext;

	public DataAccess(ArfBlocksDependencyProvider dbContextProvider)
	{
		_dbContext = dbContextProvider.GetInstance<LivestockTradingModuleDbContext>();
	}

	public async Task<(List<BoostPackage> Packages, XPageResponse Page)> All(
		string languageCode,
		XSorting sorting,
		List<XFilterItem> filters,
		XPageRequest pageRequest,
		CancellationToken ct)
	{
		var query = _dbContext.BoostPackages
			.AsNoTracking()
			.Where(p => !p.IsDeleted && p.IsActive)
			.Sort(sorting)
			.Filter(filters);

		if (sorting == null)
			query = query.OrderBy(p => p.SortOrder);

		var page = query.GetPage(pageRequest);
		var packages = await query.Paginate(page).ToListAsync(ct);

		return (packages, page);
	}
}
