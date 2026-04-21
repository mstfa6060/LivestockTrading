using LivestockTrading.Domain.Entities;
using LivestockTrading.Infrastructure.RelationalDB;
using Microsoft.EntityFrameworkCore;

namespace LivestockTrading.Application.RequestHandlers.ProductBoosts.Queries.All;

public class DataAccess : IDataAccess
{
	private readonly LivestockTradingModuleDbContext _dbContext;

	public DataAccess(ArfBlocksDependencyProvider dbContextProvider)
	{
		_dbContext = dbContextProvider.GetInstance<LivestockTradingModuleDbContext>();
	}

	public async Task<(List<ProductBoost> Boosts, XPageResponse Page)> All(
		Guid sellerId,
		XSorting sorting,
		List<XFilterItem> filters,
		XPageRequest pageRequest,
		CancellationToken ct)
	{
		var query = _dbContext.ProductBoosts
			.AsNoTracking()
			.Include(b => b.Product)
			.Include(b => b.BoostPackage)
			.Where(b => !b.IsDeleted && b.SellerId == sellerId)
			.Sort(sorting)
			.Filter(filters);

		if (sorting == null)
			query = query.OrderByDescending(b => b.CreatedAt);

		var page = query.GetPage(pageRequest);
		var boosts = await query.Paginate(page).ToListAsync(ct);

		return (boosts, page);
	}
}
