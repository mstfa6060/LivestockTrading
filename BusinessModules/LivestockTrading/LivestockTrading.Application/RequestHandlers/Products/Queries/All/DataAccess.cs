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

	public async Task<Dictionary<string, Currency>> GetCurrencyRates(CancellationToken ct)
	{
		return await _dbContext.Currencies
			.AsNoTracking()
			.Where(c => !c.IsDeleted && c.IsActive)
			.ToDictionaryAsync(c => c.Code, c => c, ct);
	}

	public async Task<(List<Product> Products, XPageResponse Page)> All(
		string countryCode,
		Guid? categoryId,
		XSorting sorting,
		List<XFilterItem> filters,
		XPageRequest pageRequest,
		bool includeDeleted,
		CancellationToken ct)
	{
		var query = _dbContext.Products
			.AsNoTracking()
			.Include(p => p.Location)
			.Where(p => includeDeleted || (!p.IsDeleted && p.Status == ProductStatus.Active));

		// Ülke filtresi
		if (!string.IsNullOrWhiteSpace(countryCode))
			query = query.Where(p => p.Location != null && p.Location.CountryCode == countryCode);

		// Kategori filtresi - üst kategori seçilmişse alt kategorilerin ürünleri de dahil edilir
		if (categoryId.HasValue)
		{
			query = query.Where(p =>
				p.CategoryId == categoryId.Value ||
				_dbContext.Categories
					.Where(c => c.ParentCategoryId == categoryId.Value && !c.IsDeleted)
					.Select(c => c.Id)
					.Contains(p.CategoryId));
		}

		query = query.Sort(sorting).Filter(filters);

		if (sorting == null)
			query = query.OrderByDescending(p => p.BoostScore > 0 && p.FeaturedUntil > DateTime.UtcNow)
				.ThenByDescending(p => p.BoostScore)
				.ThenByDescending(p => p.CreatedAt);

		var page = query.GetPage(pageRequest);
		var products = await query.Paginate(page).ToListAsync(ct);

		return (products, page);
	}
}
