using LivestockTrading.Domain.Entities;
using LivestockTrading.Infrastructure.RelationalDB;
using Microsoft.EntityFrameworkCore;

namespace LivestockTrading.Application.RequestHandlers.Transporters.Queries.Pick;

public class DataAccess : IDataAccess
{
	private readonly LivestockTradingModuleDbContext _dbContext;

	public DataAccess(ArfBlocksDependencyProvider dbContextProvider)
	{
		_dbContext = dbContextProvider.GetInstance<LivestockTradingModuleDbContext>();
	}

	public async Task<List<Transporter>> Pick(
		List<Guid> selectedIds,
		string keyword,
		int limit,
		CancellationToken ct)
	{
		var query = _dbContext.Transporters
			.AsNoTracking()
			.Where(t => !t.IsDeleted && t.IsActive);

		if (selectedIds != null && selectedIds.Any())
		{
			return await query
				.Where(t => selectedIds.Contains(t.Id))
				.OrderByDescending(t => t.CreatedAt)
				.ToListAsync(ct);
		}

		if (!string.IsNullOrWhiteSpace(keyword))
		{
			var lowerKeyword = keyword.ToLower();
			query = query.Where(t =>
				t.CompanyName.ToLower().Contains(lowerKeyword) ||
				t.ContactPerson.ToLower().Contains(lowerKeyword));
		}

		return await query
			.OrderByDescending(t => t.CreatedAt)
			.Take(limit > 0 ? limit : 10)
			.ToListAsync(ct);
	}
}
