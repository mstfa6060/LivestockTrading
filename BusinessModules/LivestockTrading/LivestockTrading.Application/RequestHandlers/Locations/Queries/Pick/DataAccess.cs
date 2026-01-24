using LivestockTrading.Domain.Entities;
using LivestockTrading.Infrastructure.RelationalDB;
using Microsoft.EntityFrameworkCore;

namespace LivestockTrading.Application.RequestHandlers.Locations.Queries.Pick;

public class DataAccess : IDataAccess
{
	private readonly LivestockTradingModuleDbContext _dbContext;

	public DataAccess(ArfBlocksDependencyProvider dbContextProvider)
	{
		_dbContext = dbContextProvider.GetInstance<LivestockTradingModuleDbContext>();
	}

	public async Task<List<Location>> Pick(
		List<Guid> selectedIds,
		string keyword,
		int limit,
		CancellationToken ct)
	{
		var query = _dbContext.Locations
			.AsNoTracking()
			.Where(l => !l.IsDeleted && l.IsActive);

		// Secili ID'ler varsa oncelikli olarak getir
		if (selectedIds != null && selectedIds.Any())
		{
			return await query
				.Where(l => selectedIds.Contains(l.Id))
				.OrderByDescending(l => l.CreatedAt)
				.ToListAsync(ct);
		}

		// Keyword aramasi
		if (!string.IsNullOrWhiteSpace(keyword))
		{
			var lowerKeyword = keyword.ToLower();
			query = query.Where(l =>
				l.Name.ToLower().Contains(lowerKeyword) ||
				l.City.ToLower().Contains(lowerKeyword) ||
				l.State.ToLower().Contains(lowerKeyword));
		}

		return await query
			.OrderByDescending(l => l.CreatedAt)
			.Take(limit > 0 ? limit : 10)
			.ToListAsync(ct);
	}
}
