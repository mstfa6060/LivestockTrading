using LivestockTrading.Domain.Entities;
using LivestockTrading.Infrastructure.RelationalDB;
using Microsoft.EntityFrameworkCore;

namespace LivestockTrading.Application.RequestHandlers.Categories.Queries.All;

public class DataAccess : IDataAccess
{
	private readonly LivestockTradingModuleDbContext _dbContext;

	public DataAccess(ArfBlocksDependencyProvider dbContextProvider)
	{
		_dbContext = dbContextProvider.GetInstance<LivestockTradingModuleDbContext>();
	}

	public async Task<(List<Category> Categories, Dictionary<Guid, int> ProductCounts, XPageResponse Page)> All(
		XSorting sorting,
		List<XFilterItem> filters,
		XPageRequest pageRequest,
		CancellationToken ct)
	{
		var query = _dbContext.Categories
			.Include(c => c.SubCategories)
			.AsNoTracking()
			.Where(c => !c.IsDeleted)
			.Sort(sorting)
			.Filter(filters);

		// Default sorting
		if (sorting == null)
			query = query.OrderByDescending(c => c.CreatedAt);

		// Pagination
		var page = query.GetPage(pageRequest);
		var categories = await query.Paginate(page).ToListAsync(ct);

		// Product count per category (only Active products)
		var productCounts = await _dbContext.Products
			.AsNoTracking()
			.Where(p => !p.IsDeleted && p.Status == ProductStatus.Active)
			.GroupBy(p => p.CategoryId)
			.Select(g => new { CategoryId = g.Key, Count = g.Count() })
			.ToDictionaryAsync(x => x.CategoryId, x => x.Count, ct);

		return (categories, productCounts, page);
	}
}
