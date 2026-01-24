using LivestockTrading.Domain.Entities;
using LivestockTrading.Infrastructure.RelationalDB;
using Microsoft.EntityFrameworkCore;

namespace LivestockTrading.Application.RequestHandlers.Brands.Queries.Detail;

public class DataAccess : IDataAccess
{
	private readonly LivestockTradingModuleDbContext _dbContext;

	public DataAccess(ArfBlocksDependencyProvider dbContextProvider)
	{
		_dbContext = dbContextProvider.GetInstance<LivestockTradingModuleDbContext>();
	}

	public async Task<Brand> GetById(Guid id, CancellationToken ct)
	{
		return await _dbContext.Brands
			.AsNoTracking()
			.Include(b => b.Products.Where(p => !p.IsDeleted))
			.FirstOrDefaultAsync(b => b.Id == id && !b.IsDeleted, ct);
	}
}
