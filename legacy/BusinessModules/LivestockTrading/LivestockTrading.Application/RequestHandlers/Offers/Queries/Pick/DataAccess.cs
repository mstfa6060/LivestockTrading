using LivestockTrading.Domain.Entities;
using LivestockTrading.Infrastructure.RelationalDB;
using Microsoft.EntityFrameworkCore;

namespace LivestockTrading.Application.RequestHandlers.Offers.Queries.Pick;

public class DataAccess : IDataAccess
{
	private readonly LivestockTradingModuleDbContext _dbContext;

	public DataAccess(ArfBlocksDependencyProvider dbContextProvider)
	{
		_dbContext = dbContextProvider.GetInstance<LivestockTradingModuleDbContext>();
	}

	public async Task<List<Offer>> Pick(
		List<Guid> selectedIds,
		string keyword,
		int limit,
		CancellationToken ct)
	{
		var query = _dbContext.Offers
			.AsNoTracking()
			.Where(o => !o.IsDeleted);

		// Secili ID'ler varsa oncelikli olarak getir
		if (selectedIds != null && selectedIds.Any())
		{
			return await query
				.Where(o => selectedIds.Contains(o.Id))
				.OrderByDescending(o => o.CreatedAt)
				.ToListAsync(ct);
		}

		// Keyword aramasi
		if (!string.IsNullOrWhiteSpace(keyword))
		{
			var lowerKeyword = keyword.ToLower();
			query = query.Where(o =>
				o.Message.ToLower().Contains(lowerKeyword));
		}

		return await query
			.OrderByDescending(o => o.CreatedAt)
			.Take(limit > 0 ? limit : 10)
			.ToListAsync(ct);
	}
}
