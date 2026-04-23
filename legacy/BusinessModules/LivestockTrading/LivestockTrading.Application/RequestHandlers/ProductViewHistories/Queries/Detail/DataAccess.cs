using LivestockTrading.Domain.Entities;
using LivestockTrading.Infrastructure.RelationalDB;
using Microsoft.EntityFrameworkCore;

namespace LivestockTrading.Application.RequestHandlers.ProductViewHistories.Queries.Detail;

public class DataAccess : IDataAccess
{
	private readonly LivestockTradingModuleDbContext _dbContext;

	public DataAccess(ArfBlocksDependencyProvider dbContextProvider)
	{
		_dbContext = dbContextProvider.GetInstance<LivestockTradingModuleDbContext>();
	}

	public async Task<ProductViewHistory> GetById(Guid id, CancellationToken ct)
	{
		return await _dbContext.ProductViewHistories
			.AsNoTracking()
			.FirstOrDefaultAsync(e => e.Id == id && !e.IsDeleted, ct);
	}
}
