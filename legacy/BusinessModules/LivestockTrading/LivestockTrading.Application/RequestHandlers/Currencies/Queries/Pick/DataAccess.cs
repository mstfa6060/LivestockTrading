using LivestockTrading.Domain.Entities;
using LivestockTrading.Infrastructure.RelationalDB;
using Microsoft.EntityFrameworkCore;

namespace LivestockTrading.Application.RequestHandlers.Currencies.Queries.Pick;

public class DataAccess : IDataAccess
{
	private readonly LivestockTradingModuleDbContext _dbContext;

	public DataAccess(ArfBlocksDependencyProvider dbContextProvider)
	{
		_dbContext = dbContextProvider.GetInstance<LivestockTradingModuleDbContext>();
	}

	public async Task<List<Currency>> Pick(
		List<Guid> selectedIds,
		string keyword,
		int limit,
		CancellationToken ct)
	{
		var query = _dbContext.Currencies
			.AsNoTracking()
			.Where(c => !c.IsDeleted && c.IsActive);

		// Secili ID'ler varsa oncelikli olarak getir
		if (selectedIds != null && selectedIds.Any())
		{
			return await query
				.Where(c => selectedIds.Contains(c.Id))
				.OrderByDescending(c => c.CreatedAt)
				.ToListAsync(ct);
		}

		// Keyword aramasi
		if (!string.IsNullOrWhiteSpace(keyword))
		{
			var lowerKeyword = keyword.ToLower();
			query = query.Where(c =>
				c.Code.ToLower().Contains(lowerKeyword) ||
				c.Name.ToLower().Contains(lowerKeyword) ||
				c.Symbol.ToLower().Contains(lowerKeyword));
		}

		return await query
			.OrderByDescending(c => c.CreatedAt)
			.Take(limit > 0 ? limit : 10)
			.ToListAsync(ct);
	}
}
