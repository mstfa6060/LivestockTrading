using LivestockTrading.Domain.Entities;
using LivestockTrading.Infrastructure.RelationalDB;
using Microsoft.EntityFrameworkCore;

namespace LivestockTrading.Application.RequestHandlers.Farms.Queries.Detail;

public class DataAccess : IDataAccess
{
	private readonly LivestockTradingModuleDbContext _dbContext;

	public DataAccess(ArfBlocksDependencyProvider dbContextProvider)
	{
		_dbContext = dbContextProvider.GetInstance<LivestockTradingModuleDbContext>();
	}

	public async Task<Farm> GetById(Guid id, CancellationToken ct)
	{
		return await _dbContext.Farms
			.AsNoTracking()
			.Include(f => f.Seller)
			.Include(f => f.Location)
			.FirstOrDefaultAsync(f => f.Id == id && !f.IsDeleted, ct);
	}
}
