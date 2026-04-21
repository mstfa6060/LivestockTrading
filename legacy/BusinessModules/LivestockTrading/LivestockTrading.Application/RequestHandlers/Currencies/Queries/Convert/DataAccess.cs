using LivestockTrading.Domain.Entities;
using LivestockTrading.Infrastructure.RelationalDB;
using Microsoft.EntityFrameworkCore;

namespace LivestockTrading.Application.RequestHandlers.Currencies.Queries.Convert;

public class DataAccess : IDataAccess
{
	private readonly LivestockTradingModuleDbContext _dbContext;

	public DataAccess(ArfBlocksDependencyProvider dbContextProvider)
	{
		_dbContext = dbContextProvider.GetInstance<LivestockTradingModuleDbContext>();
	}

	public async Task<Currency> GetByCode(string code, CancellationToken ct)
	{
		return await _dbContext.Currencies
			.AsNoTracking()
			.FirstOrDefaultAsync(c => c.Code == code && !c.IsDeleted && c.IsActive, ct);
	}
}
