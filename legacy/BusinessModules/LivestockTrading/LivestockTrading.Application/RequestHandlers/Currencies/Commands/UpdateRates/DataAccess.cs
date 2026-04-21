using LivestockTrading.Domain.Entities;
using LivestockTrading.Infrastructure.RelationalDB;
using Microsoft.EntityFrameworkCore;

namespace LivestockTrading.Application.RequestHandlers.Currencies.Commands.UpdateRates;

public class DataAccess : IDataAccess
{
	private readonly LivestockTradingModuleDbContext _dbContext;

	public DataAccess(ArfBlocksDependencyProvider dependencyProvider)
	{
		_dbContext = dependencyProvider.GetInstance<LivestockTradingModuleDbContext>();
	}

	public async Task<List<Currency>> GetActiveCurrencies(CancellationToken ct)
	{
		return await _dbContext.Currencies
			.Where(c => !c.IsDeleted && c.IsActive)
			.ToListAsync(ct);
	}

	public async Task SaveChanges(CancellationToken ct)
	{
		await _dbContext.SaveChangesAsync(ct);
	}
}
