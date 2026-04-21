using LivestockTrading.Domain.Entities;
using LivestockTrading.Infrastructure.RelationalDB;
using Microsoft.EntityFrameworkCore;

namespace LivestockTrading.Application.RequestHandlers.Currencies.Queries.Detail;

public class DataAccess : IDataAccess
{
	private readonly LivestockTradingModuleDbContext _dbContext;

	public DataAccess(ArfBlocksDependencyProvider dbContextProvider)
	{
		_dbContext = dbContextProvider.GetInstance<LivestockTradingModuleDbContext>();
	}

	public async Task<Currency> GetById(Guid id, CancellationToken ct)
	{
		return await _dbContext.Currencies
			.AsNoTracking()
			.FirstOrDefaultAsync(c => c.Id == id && !c.IsDeleted, ct);
	}
}
