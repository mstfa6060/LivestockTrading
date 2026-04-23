using LivestockTrading.Domain.Entities;
using LivestockTrading.Infrastructure.RelationalDB;
using Microsoft.EntityFrameworkCore;

namespace LivestockTrading.Application.RequestHandlers.Languages.Queries.All;

public class DataAccess : IDataAccess
{
	private readonly LivestockTradingModuleDbContext _dbContext;

	public DataAccess(ArfBlocksDependencyProvider dbContextProvider)
	{
		_dbContext = dbContextProvider.GetInstance<LivestockTradingModuleDbContext>();
	}

	public async Task<(List<Language> Languages, XPageResponse Page)> All(
		XSorting sorting,
		List<XFilterItem> filters,
		XPageRequest pageRequest,
		CancellationToken ct)
	{
		var query = _dbContext.Languages
			.AsNoTracking()
			.Where(l => !l.IsDeleted)
			.Sort(sorting)
			.Filter(filters);

		// Default sorting
		if (sorting == null)
			query = query.OrderByDescending(l => l.CreatedAt);

		// Pagination
		var page = query.GetPage(pageRequest);
		var languages = await query.Paginate(page).ToListAsync(ct);

		return (languages, page);
	}
}
