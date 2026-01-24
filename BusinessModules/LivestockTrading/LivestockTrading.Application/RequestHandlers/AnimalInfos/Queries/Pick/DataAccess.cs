using LivestockTrading.Domain.Entities;
using LivestockTrading.Infrastructure.RelationalDB;
using Microsoft.EntityFrameworkCore;

namespace LivestockTrading.Application.RequestHandlers.AnimalInfos.Queries.Pick;

public class DataAccess : IDataAccess
{
	private readonly LivestockTradingModuleDbContext _dbContext;

	public DataAccess(ArfBlocksDependencyProvider dbContextProvider)
	{
		_dbContext = dbContextProvider.GetInstance<LivestockTradingModuleDbContext>();
	}

	public async Task<List<AnimalInfo>> Pick(
		List<Guid> selectedIds,
		string keyword,
		int limit,
		CancellationToken ct)
	{
		var query = _dbContext.AnimalInfos
			.AsNoTracking()
			.Where(e => !e.IsDeleted);

		// Secili ID'ler varsa oncelikli olarak getir
		if (selectedIds != null && selectedIds.Any())
		{
			return await query
				.Where(e => selectedIds.Contains(e.Id))
				.OrderByDescending(e => e.CreatedAt)
				.ToListAsync(ct);
		}

		// Keyword aramasi
		if (!string.IsNullOrWhiteSpace(keyword))
		{
			var lowerKeyword = keyword.ToLower();
			query = query.Where(e =>
				e.BreedName.ToLower().Contains(lowerKeyword) ||
				e.TagNumber.ToLower().Contains(lowerKeyword) ||
				e.MicrochipNumber.ToLower().Contains(lowerKeyword));
		}

		return await query
			.OrderByDescending(e => e.CreatedAt)
			.Take(limit > 0 ? limit : 10)
			.ToListAsync(ct);
	}
}
