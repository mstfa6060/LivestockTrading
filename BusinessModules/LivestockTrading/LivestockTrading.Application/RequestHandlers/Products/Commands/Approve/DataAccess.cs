using LivestockTrading.Domain.Entities;
using LivestockTrading.Infrastructure.RelationalDB;
using Microsoft.EntityFrameworkCore;

namespace LivestockTrading.Application.RequestHandlers.Products.Commands.Approve;

public class DataAccess : IDataAccess
{
	private readonly LivestockTradingModuleDbContext _dbContext;

	public DataAccess(ArfBlocksDependencyProvider dependencyProvider)
	{
		_dbContext = dependencyProvider.GetInstance<LivestockTradingModuleDbContext>();
	}

	public async Task<Product> GetProductById(Guid productId)
	{
		return await _dbContext.Products
			.FirstOrDefaultAsync(p => p.Id == productId && !p.IsDeleted);
	}

	public async Task SaveChanges()
	{
		await _dbContext.SaveChangesAsync();
	}
}
