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
		string currency,
		string sortBy,
		XSorting sorting,
		XPageRequest pageRequest,
		CancellationToken ct)
	{
		var dbQuery = _dbContext.Products
			.AsNoTracking()
			.Include(p => p.Location)
			.Include(p => p.Category)
			.Include(p => p.Seller)
			.Where(p => !p.IsDeleted && p.Status == ProductStatus.Active);

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

		// Kategori filtresi
		if (categoryId.HasValue)
			dbQuery = dbQuery.Where(p => p.CategoryId == categoryId.Value);

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

		// Para birimi filtresi
		if (!string.IsNullOrWhiteSpace(currency))
			dbQuery = dbQuery.Where(p => p.Currency == currency);

		// Sıralama
		if (!string.IsNullOrWhiteSpace(sortBy))
		{
			dbQuery = sortBy.ToLower() switch
			{
				"price_asc" => dbQuery.OrderBy(p => p.BasePrice),
				"price_desc" => dbQuery.OrderByDescending(p => p.BasePrice),
				"newest" => dbQuery.OrderByDescending(p => p.CreatedAt),
				"most_viewed" => dbQuery.OrderByDescending(p => p.ViewCount),
				_ => dbQuery.OrderByDescending(p => p.CreatedAt) // "relevance" or default
			};
		}
		else
		{
			dbQuery = dbQuery.Sort(sorting);

			if (sorting == null)
				dbQuery = dbQuery.OrderByDescending(p => p.CreatedAt);
		}

		var page = dbQuery.GetPage(pageRequest);
		var products = await dbQuery.Paginate(page).ToListAsync(ct);

		return (products, page);
	}
}
