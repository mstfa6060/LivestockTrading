using LivestockTrading.Domain.Entities;
using LivestockTrading.Infrastructure.RelationalDB;
using Microsoft.EntityFrameworkCore;

namespace LivestockTrading.Application.RequestHandlers.Brands.Commands.Delete;

public class DataAccess : IDataAccess
{
	private readonly LivestockTradingModuleDbContext _dbContext;

	public DataAccess(ArfBlocksDependencyProvider dependencyProvider)
	{
		_dbContext = dependencyProvider.GetInstance<LivestockTradingModuleDbContext>();
	}

	public async Task<Brand> GetBrandById(Guid brandId)
	{
		return await _dbContext.Brands
			.FirstOrDefaultAsync(b => b.Id == brandId && !b.IsDeleted);
	}

	public async Task SaveChanges()
	{
		await _dbContext.SaveChangesAsync();
	}
}
