using LivestockTrading.Domain.Entities;
using LivestockTrading.Infrastructure.RelationalDB;
using Microsoft.EntityFrameworkCore;

namespace LivestockTrading.Application.RequestHandlers.ShippingRates.Queries.Pick;

public class DataAccess : IDataAccess
{
	private readonly LivestockTradingModuleDbContext _dbContext;

	public DataAccess(ArfBlocksDependencyProvider dependencyProvider)
	{
		_dbContext = dependencyProvider.GetInstance<LivestockTradingModuleDbContext>();
	}

	public async Task<List<ShippingRate>> Pick(
		List<Guid> selectedIds,
		string keyword,
		int limit,
		CancellationToken ct)
	{
		var query = _dbContext.ShippingRates
			.AsNoTracking()
			.Where(e => !e.IsDeleted && e.IsActive);

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
				e.Currency.ToLower().Contains(lowerKeyword));
		}

		return await query
			.OrderByDescending(e => e.CreatedAt)
			.Take(limit > 0 ? limit : 10)
			.ToListAsync(ct);
	}
}
