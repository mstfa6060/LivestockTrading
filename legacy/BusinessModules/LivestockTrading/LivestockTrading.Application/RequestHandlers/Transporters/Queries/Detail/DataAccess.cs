using LivestockTrading.Domain.Entities;
using LivestockTrading.Infrastructure.RelationalDB;
using Microsoft.EntityFrameworkCore;

namespace LivestockTrading.Application.RequestHandlers.Transporters.Queries.Detail;

public class DataAccess : IDataAccess
{
	private readonly LivestockTradingModuleDbContext _dbContext;

	public DataAccess(ArfBlocksDependencyProvider dbContextProvider)
	{
		_dbContext = dbContextProvider.GetInstance<LivestockTradingModuleDbContext>();
	}

	public async Task<Transporter> GetById(Guid id, CancellationToken ct)
	{
		return await _dbContext.Transporters
			.AsNoTracking()
			.FirstOrDefaultAsync(t => t.Id == id && !t.IsDeleted, ct);
	}
}
