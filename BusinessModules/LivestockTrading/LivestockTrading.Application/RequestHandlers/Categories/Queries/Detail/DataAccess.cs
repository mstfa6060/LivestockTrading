using LivestockTrading.Domain.Entities;
using LivestockTrading.Infrastructure.RelationalDB;
using Microsoft.EntityFrameworkCore;

namespace LivestockTrading.Application.RequestHandlers.Categories.Queries.Detail;

public class DataAccess : IDataAccess
{
	private readonly LivestockTradingModuleDbContext _dbContext;

	public DataAccess(ArfBlocksDependencyProvider dbContextProvider)
	{
		_dbContext = dbContextProvider.GetInstance<LivestockTradingModuleDbContext>();
	}

	public async Task<Category> GetById(Guid id, CancellationToken ct)
	{
		return await _dbContext.Categories
			.AsNoTracking()
			.Include(c => c.ParentCategory)
			.Include(c => c.SubCategories.Where(sc => !sc.IsDeleted))
			.FirstOrDefaultAsync(c => c.Id == id && !c.IsDeleted, ct);
	}
}
