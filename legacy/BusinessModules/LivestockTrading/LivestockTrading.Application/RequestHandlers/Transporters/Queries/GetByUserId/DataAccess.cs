using LivestockTrading.Domain.Entities;
using LivestockTrading.Infrastructure.RelationalDB;
using Microsoft.EntityFrameworkCore;

namespace LivestockTrading.Application.RequestHandlers.Transporters.Queries.GetByUserId;

public class DataAccess : IDataAccess
{
	private readonly LivestockTradingModuleDbContext _dbContext;

	public DataAccess(ArfBlocksDependencyProvider dbContextProvider)
	{
		_dbContext = dbContextProvider.GetInstance<LivestockTradingModuleDbContext>();
	}

	public async Task<Transporter> GetByUserId(Guid userId, CancellationToken ct)
	{
		return await _dbContext.Transporters
			.AsNoTracking()
			.FirstOrDefaultAsync(t => t.UserId == userId && !t.IsDeleted, ct);
	}
}
