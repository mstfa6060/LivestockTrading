using LivestockTrading.Domain.Entities;
using LivestockTrading.Infrastructure.RelationalDB;
using Microsoft.EntityFrameworkCore;

namespace LivestockTrading.Application.RequestHandlers.ProductReports.Commands.Create;

public class DataAccess : IDataAccess
{
	private readonly LivestockTradingModuleDbContext _dbContext;

	public DataAccess(ArfBlocksDependencyProvider dependencyProvider)
	{
		_dbContext = dependencyProvider.GetInstance<LivestockTradingModuleDbContext>();
	}

	public async Task Add(ProductReport entity)
	{
		_dbContext.ProductReports.Add(entity);
		await _dbContext.SaveChangesAsync();
	}

	public async Task<bool> CheckDuplicate(Guid productId, Guid reporterUserId)
	{
		return await _dbContext.ProductReports
			.AsNoTracking()
			.AnyAsync(r => r.ProductId == productId && r.ReporterUserId == reporterUserId && !r.IsDeleted);
	}
}
