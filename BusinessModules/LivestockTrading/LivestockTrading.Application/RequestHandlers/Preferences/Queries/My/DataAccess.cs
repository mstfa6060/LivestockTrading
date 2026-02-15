using LivestockTrading.Domain.Entities;
using LivestockTrading.Infrastructure.RelationalDB;
using Microsoft.EntityFrameworkCore;

namespace LivestockTrading.Application.RequestHandlers.Preferences.Queries.My;

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
			.AsNoTracking()
			.FirstOrDefaultAsync(e => e.UserId == userId && !e.IsDeleted, ct);
	}

	public async Task Add(Domain.Entities.UserPreferences entity, CancellationToken ct)
	{
		_dbContext.UserPreferences.Add(entity);
		await _dbContext.SaveChangesAsync(ct);
	}
}
