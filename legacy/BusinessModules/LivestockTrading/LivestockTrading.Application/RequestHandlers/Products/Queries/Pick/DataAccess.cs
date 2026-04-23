using LivestockTrading.Domain.Entities;
using LivestockTrading.Infrastructure.RelationalDB;
using Microsoft.EntityFrameworkCore;

namespace LivestockTrading.Application.RequestHandlers.Products.Queries.Pick;

public class DataAccess : IDataAccess
{
	private readonly LivestockTradingModuleDbContext _dbContext;

	public DataAccess(ArfBlocksDependencyProvider dbContextProvider)
	{
		_dbContext = dbContextProvider.GetInstance<LivestockTradingModuleDbContext>();
	}

	public async Task<List<Product>> Pick(
		List<Guid> selectedIds,
		string keyword,
		int limit,
		CancellationToken ct)
	{
		var query = _dbContext.Products
			.AsNoTracking()
			.Where(p => !p.IsDeleted);

		// Secili ID'ler varsa oncelikli olarak getir
		if (selectedIds != null && selectedIds.Any())
		{
			return await query
				.Where(p => selectedIds.Contains(p.Id))
				.OrderByDescending(p => p.CreatedAt)
				.ToListAsync(ct);
		}

		// Keyword aramasi
		if (!string.IsNullOrWhiteSpace(keyword))
		{
			var lowerKeyword = keyword.ToLower();
			query = query.Where(p =>
				p.Title.ToLower().Contains(lowerKeyword) ||
				p.Slug.ToLower().Contains(lowerKeyword) ||
				p.Description.ToLower().Contains(lowerKeyword));
		}

		return await query
			.OrderByDescending(p => p.CreatedAt)
			.Take(limit > 0 ? limit : 10)
			.ToListAsync(ct);
	}
}
