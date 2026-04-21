using LivestockTrading.Domain.Entities;
using LivestockTrading.Infrastructure.RelationalDB;
using Microsoft.EntityFrameworkCore;

namespace LivestockTrading.Application.RequestHandlers.AppVersions.Queries.Check;

public class DataAccess : IDataAccess
{
	private readonly LivestockTradingModuleDbContext _dbContext;

	public DataAccess(ArfBlocksDependencyProvider dbContextProvider)
	{
		_dbContext = dbContextProvider.GetInstance<LivestockTradingModuleDbContext>();
	}

	public async Task<AppVersionConfig> GetActiveByPlatform(int platform, CancellationToken ct)
	{
		return await _dbContext.AppVersionConfigs
			.AsNoTracking()
			.Where(e => !e.IsDeleted && e.Platform == platform)
			.OrderByDescending(e => e.CreatedAt)
			.FirstOrDefaultAsync(ct);
	}
}
