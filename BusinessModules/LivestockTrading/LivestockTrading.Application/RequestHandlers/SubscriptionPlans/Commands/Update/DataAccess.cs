using LivestockTrading.Domain.Entities;
using LivestockTrading.Infrastructure.RelationalDB;
using Microsoft.EntityFrameworkCore;

namespace LivestockTrading.Application.RequestHandlers.SubscriptionPlans.Commands.Update;

public class DataAccess : IDataAccess
{
	private readonly LivestockTradingModuleDbContext _dbContext;

	public DataAccess(ArfBlocksDependencyProvider dependencyProvider)
	{
		_dbContext = dependencyProvider.GetInstance<LivestockTradingModuleDbContext>();
	}

	public async Task<SubscriptionPlan> GetById(Guid id)
	{
		return await _dbContext.SubscriptionPlans
			.FirstOrDefaultAsync(p => p.Id == id && !p.IsDeleted);
	}

	public async Task SaveChanges()
	{
		await _dbContext.SaveChangesAsync();
	}
}
