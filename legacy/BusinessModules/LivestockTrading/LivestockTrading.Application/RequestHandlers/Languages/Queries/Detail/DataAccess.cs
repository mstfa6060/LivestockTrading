using LivestockTrading.Domain.Entities;
using LivestockTrading.Infrastructure.RelationalDB;
using Microsoft.EntityFrameworkCore;

namespace LivestockTrading.Application.RequestHandlers.Languages.Queries.Detail;

public class DataAccess : IDataAccess
{
	private readonly LivestockTradingModuleDbContext _dbContext;

	public DataAccess(ArfBlocksDependencyProvider dbContextProvider)
	{
		_dbContext = dbContextProvider.GetInstance<LivestockTradingModuleDbContext>();
	}

	public async Task<Language> GetById(Guid id, CancellationToken ct)
	{
		return await _dbContext.Languages
			.AsNoTracking()
			.FirstOrDefaultAsync(l => l.Id == id && !l.IsDeleted, ct);
	}
}
