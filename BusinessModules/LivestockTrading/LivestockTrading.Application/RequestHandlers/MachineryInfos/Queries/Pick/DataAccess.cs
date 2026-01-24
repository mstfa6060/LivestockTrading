using LivestockTrading.Domain.Entities;
using LivestockTrading.Infrastructure.RelationalDB;
using Microsoft.EntityFrameworkCore;

namespace LivestockTrading.Application.RequestHandlers.MachineryInfos.Queries.Pick;

public class DataAccess : IDataAccess
{
	private readonly LivestockTradingModuleDbContext _dbContext;

	public DataAccess(ArfBlocksDependencyProvider dependencyProvider)
	{
		_dbContext = dependencyProvider.GetInstance<LivestockTradingModuleDbContext>();
	}

	public async Task<List<MachineryInfo>> Pick(
		List<Guid> selectedIds,
		string keyword,
		int limit,
		CancellationToken ct)
	{
		var query = _dbContext.MachineryInfos
			.AsNoTracking()
			.Where(e => !e.IsDeleted);

		if (selectedIds != null && selectedIds.Any())
		{
			return await query
				.Where(e => selectedIds.Contains(e.Id))
				.OrderByDescending(e => e.CreatedAt)
				.ToListAsync(ct);
		}

		if (!string.IsNullOrWhiteSpace(keyword))
		{
			var lowerKeyword = keyword.ToLower();
			query = query.Where(e =>
				(e.Model != null && e.Model.ToLower().Contains(lowerKeyword)) ||
				(e.SerialNumber != null && e.SerialNumber.ToLower().Contains(lowerKeyword)));
		}

		return await query
			.OrderByDescending(e => e.CreatedAt)
			.Take(limit > 0 ? limit : 10)
			.ToListAsync(ct);
	}
}
