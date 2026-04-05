using LivestockTrading.Infrastructure.RelationalDB;
using Microsoft.EntityFrameworkCore;

namespace LivestockTrading.Application.RequestHandlers.UserPreferences.Queries.GetTimeZone;

public class DataAccess : IDataAccess
{
	private readonly LivestockTradingModuleDbContext _dbContext;

	public DataAccess(ArfBlocksDependencyProvider dependencyProvider)
	{
		_dbContext = dependencyProvider.GetInstance<LivestockTradingModuleDbContext>();
	}

	public async Task<string> GetTimeZoneByUserId(Guid userId, CancellationToken ct)
	{
		return await _dbContext.UserPreferences
			.AsNoTracking()
			.Where(p => p.UserId == userId && !p.IsDeleted)
			.Select(p => p.TimeZone)
			.FirstOrDefaultAsync(ct);
	}
}
