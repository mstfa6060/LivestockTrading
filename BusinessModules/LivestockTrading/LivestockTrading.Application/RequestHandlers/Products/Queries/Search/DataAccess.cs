using LivestockTrading.Domain.Entities;
using LivestockTrading.Infrastructure.RelationalDB;

namespace LivestockTrading.Application.RequestHandlers.Products.Queries.Search;

public class DataAccess : IDataAccess
{
	private readonly LivestockTradingModuleDbContext _dbContext;

	public DataAccess(ArfBlocksDependencyProvider dbContextProvider)
	{
		_dbContext = dbContextProvider.GetInstance<LivestockTradingModuleDbContext>();
	}

	public async Task<(List<Product> Products, XPageResponse Page)> Search(
		string query,
		Guid? categoryId,
		Guid? brandId,
		decimal? minPrice,
		decimal? maxPrice,
		int? condition,
		string countryCode,
		string city,
		Guid? sellerId,
		XSorting sorting,
		XPageRequest pageRequest,
		bool includeDeleted,
		CancellationToken ct)
	{
		var dbQuery = _dbContext.Products
			.AsNoTracking()
			.Include(p => p.Location)
			.Where(p => includeDeleted || (!p.IsDeleted && p.Status == ProductStatus.Active));

		// Arama sorgusu - başlık, açıklama, slug'da aranır
		if (!string.IsNullOrWhiteSpace(query))
		{
			var lowerQuery = query.ToLower();
			dbQuery = dbQuery.Where(p =>
				p.Title.ToLower().Contains(lowerQuery) ||
				p.ShortDescription.ToLower().Contains(lowerQuery) ||
				p.Slug.ToLower().Contains(lowerQuery) ||
				p.MetaKeywords.ToLower().Contains(lowerQuery));
		}

		// Kategori filtresi - üst kategori seçilmişse alt kategorilerin ürünleri de dahil edilir
		if (categoryId.HasValue)
		{
			dbQuery = dbQuery.Where(p =>
				p.CategoryId == categoryId.Value ||
				_dbContext.Categories
					.Where(c => c.ParentCategoryId == categoryId.Value && !c.IsDeleted)
					.Select(c => c.Id)
					.Contains(p.CategoryId));
		}

		// Marka filtresi
		if (brandId.HasValue)
			dbQuery = dbQuery.Where(p => p.BrandId == brandId.Value);

		// Fiyat aralığı
		if (minPrice.HasValue)
			dbQuery = dbQuery.Where(p => p.BasePrice >= minPrice.Value);

		if (maxPrice.HasValue)
			dbQuery = dbQuery.Where(p => p.BasePrice <= maxPrice.Value);

		// Ürün durumu
		if (condition.HasValue)
			dbQuery = dbQuery.Where(p => (int)p.Condition == condition.Value);

		// Ülke filtresi
		if (!string.IsNullOrWhiteSpace(countryCode))
			dbQuery = dbQuery.Where(p => p.Location != null && p.Location.CountryCode == countryCode);

		// Şehir filtresi
		if (!string.IsNullOrWhiteSpace(city))
		{
			var lowerCity = city.ToLower();
			dbQuery = dbQuery.Where(p => p.Location != null && p.Location.City.ToLower().Contains(lowerCity));
		}

		// Satıcı filtresi
		if (sellerId.HasValue)
			dbQuery = dbQuery.Where(p => p.SellerId == sellerId.Value);

		// Sıralama
		dbQuery = dbQuery.Sort(sorting);

		if (sorting == null)
			dbQuery = dbQuery.OrderByDescending(p => p.CreatedAt);

		var page = dbQuery.GetPage(pageRequest);
		var products = await dbQuery.Paginate(page).ToListAsync(ct);

		return (products, page);
	}
}
