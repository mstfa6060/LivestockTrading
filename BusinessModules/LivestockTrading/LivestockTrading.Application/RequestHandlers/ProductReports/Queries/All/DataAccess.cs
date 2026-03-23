using LivestockTrading.Domain.Entities;
using LivestockTrading.Infrastructure.RelationalDB;
using Microsoft.EntityFrameworkCore;

namespace LivestockTrading.Application.RequestHandlers.ProductReports.Queries.All;

public class DataAccess : IDataAccess
{
	private readonly LivestockTradingModuleDbContext _dbContext;

	public DataAccess(ArfBlocksDependencyProvider dependencyProvider)
	{
		_dbContext = dependencyProvider.GetInstance<LivestockTradingModuleDbContext>();
	}

	public async Task<(List<ProductReport> Reports, XPageResponse Page)> All(
		XSorting sorting,
		List<XFilterItem> filters,
		XPageRequest pageRequest,
		CancellationToken ct)
	{
		var query = _dbContext.ProductReports
			.AsNoTracking()
			.Where(r => !r.IsDeleted);

		query = query.Sort(sorting).Filter(filters);

		if (sorting == null)
			query = query.OrderByDescending(r => r.CreatedAt);

		var page = query.GetPage(pageRequest);
		var reports = await query.Paginate(page).ToListAsync(ct);

		return (reports, page);
	}
}
