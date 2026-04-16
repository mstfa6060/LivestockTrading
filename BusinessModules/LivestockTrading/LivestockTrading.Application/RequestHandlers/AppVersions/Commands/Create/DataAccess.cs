using LivestockTrading.Domain.Entities;
using LivestockTrading.Infrastructure.RelationalDB;

namespace LivestockTrading.Application.RequestHandlers.AppVersions.Commands.Create;

public class DataAccess : IDataAccess
{
	private readonly LivestockTradingModuleDbContext _dbContext;

	public DataAccess(ArfBlocksDependencyProvider dependencyProvider)
	{
		_dbContext = dependencyProvider.GetInstance<LivestockTradingModuleDbContext>();
	}

	public async Task Add(AppVersionConfig entity)
	{
		_dbContext.AppVersionConfigs.Add(entity);
		await _dbContext.SaveChangesAsync();
	}
}
