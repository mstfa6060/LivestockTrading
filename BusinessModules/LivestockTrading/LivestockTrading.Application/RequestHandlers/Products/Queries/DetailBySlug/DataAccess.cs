using LivestockTrading.Domain.Entities;
using LivestockTrading.Infrastructure.RelationalDB;
using Microsoft.EntityFrameworkCore;

namespace LivestockTrading.Application.RequestHandlers.Products.Queries.DetailBySlug;

public class DataAccess : IDataAccess
{
	private readonly LivestockTradingModuleDbContext _dbContext;

	public DataAccess(ArfBlocksDependencyProvider dbContextProvider)
	{
		_dbContext = dbContextProvider.GetInstance<LivestockTradingModuleDbContext>();
	}

	public async Task<Product> GetBySlug(string slug, CancellationToken ct)
	{
		return await _dbContext.Products
			.AsNoTracking()
			.Include(p => p.Category)
			.Include(p => p.Brand)
			.Include(p => p.Seller)
			.Include(p => p.Location)
			.FirstOrDefaultAsync(p => p.Slug == slug && !p.IsDeleted, ct);
	}
}
