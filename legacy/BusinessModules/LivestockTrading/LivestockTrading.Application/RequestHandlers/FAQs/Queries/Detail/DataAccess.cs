using LivestockTrading.Domain.Entities;
using LivestockTrading.Infrastructure.RelationalDB;
using Microsoft.EntityFrameworkCore;

namespace LivestockTrading.Application.RequestHandlers.FAQs.Queries.Detail;

public class DataAccess : IDataAccess
{
	private readonly LivestockTradingModuleDbContext _dbContext;

	public DataAccess(ArfBlocksDependencyProvider dbContextProvider)
	{
		_dbContext = dbContextProvider.GetInstance<LivestockTradingModuleDbContext>();
	}

	public async Task<FAQ> GetById(Guid id, CancellationToken ct)
	{
		return await _dbContext.FAQs
			.AsNoTracking()
			.FirstOrDefaultAsync(e => e.Id == id && !e.IsDeleted, ct);
	}
}
