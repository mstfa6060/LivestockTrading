using LivestockTrading.Domain.Entities;
using LivestockTrading.Infrastructure.RelationalDB;
using Microsoft.EntityFrameworkCore;

namespace LivestockTrading.Application.RequestHandlers.Sellers.Queries.DetailByUserId;

public class DataAccess : IDataAccess
{
	private readonly LivestockTradingModuleDbContext _dbContext;

	public DataAccess(ArfBlocksDependencyProvider dbContextProvider)
	{
		_dbContext = dbContextProvider.GetInstance<LivestockTradingModuleDbContext>();
	}

	public async Task<Seller> GetByUserId(Guid userId, CancellationToken ct)
	{
		return await _dbContext.Sellers
			.AsNoTracking()
			.FirstOrDefaultAsync(s => s.UserId == userId && !s.IsDeleted, ct);
	}
}
