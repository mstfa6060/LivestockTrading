using LivestockTrading.Domain.Entities;
using LivestockTrading.Infrastructure.RelationalDB;
using Microsoft.EntityFrameworkCore;

namespace LivestockTrading.Application.RequestHandlers.Preferences.Commands.Update;

public class DataAccess : IDataAccess
{
	private readonly LivestockTradingModuleDbContext _dbContext;

	public DataAccess(ArfBlocksDependencyProvider dependencyProvider)
	{
		_dbContext = dependencyProvider.GetInstance<LivestockTradingModuleDbContext>();
	}

	public async Task<Domain.Entities.UserPreferences> GetByUserId(Guid userId, CancellationToken ct)
	{
		return await _dbContext.UserPreferences
			.FirstOrDefaultAsync(e => e.UserId == userId && !e.IsDeleted, ct);
	}

	public async Task Add(Domain.Entities.UserPreferences entity, CancellationToken ct)
	{
		_dbContext.UserPreferences.Add(entity);
		await _dbContext.SaveChangesAsync(ct);
	}

	public async Task SaveChanges(CancellationToken ct)
	{
		await _dbContext.SaveChangesAsync(ct);
	}
}
