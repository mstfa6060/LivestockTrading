using LivestockTrading.Domain.Entities;
using LivestockTrading.Infrastructure.RelationalDB;
using Microsoft.EntityFrameworkCore;

namespace LivestockTrading.Application.RequestHandlers.SeedInfos.Queries.Detail;

public class DataAccess : IDataAccess
{
	private readonly LivestockTradingModuleDbContext _dbContext;

	public DataAccess(ArfBlocksDependencyProvider dependencyProvider)
	{
		_dbContext = dependencyProvider.GetInstance<LivestockTradingModuleDbContext>();
	}

	public async Task<SeedInfo> GetById(Guid id, CancellationToken ct)
	{
		return await _dbContext.SeedInfos
			.AsNoTracking()
			.FirstOrDefaultAsync(e => e.Id == id && !e.IsDeleted, ct);
	}
}
