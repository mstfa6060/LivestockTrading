using LivestockTrading.Domain.Entities;
using LivestockTrading.Infrastructure.RelationalDB;
using Microsoft.EntityFrameworkCore;

namespace LivestockTrading.Application.RequestHandlers.Conversations.Queries.Detail;

public class DataAccess : IDataAccess
{
	private readonly LivestockTradingModuleDbContext _dbContext;

	public DataAccess(ArfBlocksDependencyProvider dbContextProvider)
	{
		_dbContext = dbContextProvider.GetInstance<LivestockTradingModuleDbContext>();
	}

	public async Task<Conversation> GetById(Guid id, CancellationToken ct)
	{
		return await _dbContext.Conversations
			.AsNoTracking()
			.FirstOrDefaultAsync(c => c.Id == id && !c.IsDeleted, ct);
	}
}
