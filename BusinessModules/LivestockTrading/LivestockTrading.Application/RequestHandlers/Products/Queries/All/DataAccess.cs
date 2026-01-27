using LivestockTrading.Domain.Entities;
using LivestockTrading.Infrastructure.RelationalDB;
using Microsoft.EntityFrameworkCore;

namespace LivestockTrading.Application.RequestHandlers.Products.Queries.All;

public class DataAccess : IDataAccess
{
	private readonly LivestockTradingModuleDbContext _dbContext;

	public DataAccess(ArfBlocksDependencyProvider dbContextProvider)
	{
		_dbContext = dbContextProvider.GetInstance<LivestockTradingModuleDbContext>();
	}

	public async Task<(List<Product> Products, XPageResponse Page)> All(
		string countryCode,
		XSorting sorting,
		List<XFilterItem> filters,
		XPageRequest pageRequest,
		CancellationToken ct)
	{
		var query = _dbContext.Products
			.AsNoTracking()
			.Include(p => p.Location)
			.Where(p => !p.IsDeleted);

		// Ülke filtresi - belirtilmişse sadece o ülkenin ürünlerini getir
		if (!string.IsNullOrWhiteSpace(countryCode))
		{
			query = query.Where(p => p.Location != null && p.Location.CountryCode == countryCode);
		}

		query = query.Sort(sorting).Filter(filters);

		// Default sorting
		if (sorting == null)
			query = query.OrderByDescending(p => p.CreatedAt);

		// Pagination
		var page = query.GetPage(pageRequest);
		var products = await query.Paginate(page).ToListAsync(ct);

		return (products, page);
	}
}
