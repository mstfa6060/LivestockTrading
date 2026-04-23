using LivestockTrading.Domain.Entities;
using LivestockTrading.Infrastructure.RelationalDB;
using Microsoft.EntityFrameworkCore;

namespace LivestockTrading.Application.RequestHandlers.Deals.Queries.Pick;

public class DataAccess : IDataAccess
{
	private readonly LivestockTradingModuleDbContext _dbContext;

	public DataAccess(ArfBlocksDependencyProvider dbContextProvider)
	{
		_dbContext = dbContextProvider.GetInstance<LivestockTradingModuleDbContext>();
	}

	public async Task<List<Deal>> Pick(
		List<Guid> selectedIds,
		string keyword,
		int limit,
		CancellationToken ct)
	{
		var query = _dbContext.Deals
			.AsNoTracking()
			.Where(d => !d.IsDeleted);

		if (selectedIds != null && selectedIds.Any())
		{
			return await query
				.Where(d => selectedIds.Contains(d.Id))
				.OrderByDescending(d => d.CreatedAt)
				.ToListAsync(ct);
		}

		if (!string.IsNullOrWhiteSpace(keyword))
		{
			var lowerKeyword = keyword.ToLower();
			query = query.Where(d =>
				d.DealNumber.ToLower().Contains(lowerKeyword));
		}

		return await query
			.OrderByDescending(d => d.CreatedAt)
			.Take(limit > 0 ? limit : 10)
			.ToListAsync(ct);
	}
}
