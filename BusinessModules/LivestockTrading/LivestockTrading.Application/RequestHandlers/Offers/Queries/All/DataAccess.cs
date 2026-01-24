using LivestockTrading.Domain.Entities;
using LivestockTrading.Infrastructure.RelationalDB;
using Microsoft.EntityFrameworkCore;

namespace LivestockTrading.Application.RequestHandlers.Offers.Queries.All;

public class DataAccess : IDataAccess
{
	private readonly LivestockTradingModuleDbContext _dbContext;

	public DataAccess(ArfBlocksDependencyProvider dbContextProvider)
	{
		_dbContext = dbContextProvider.GetInstance<LivestockTradingModuleDbContext>();
	}

	public async Task<(List<Offer> Offers, XPageResponse Page)> All(
		XSorting sorting,
		List<XFilterItem> filters,
		XPageRequest pageRequest,
		CancellationToken ct)
	{
		var query = _dbContext.Offers
			.AsNoTracking()
			.Where(o => !o.IsDeleted)
			.Sort(sorting)
			.Filter(filters);

		// Default sorting
		if (sorting == null)
			query = query.OrderByDescending(o => o.CreatedAt);

		// Pagination
		var page = query.GetPage(pageRequest);
		var offers = await query.Paginate(page).ToListAsync(ct);

		return (offers, page);
	}
}
