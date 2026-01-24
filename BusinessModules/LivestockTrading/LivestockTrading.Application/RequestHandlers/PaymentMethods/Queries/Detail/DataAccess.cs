using LivestockTrading.Domain.Entities;
using LivestockTrading.Infrastructure.RelationalDB;
using Microsoft.EntityFrameworkCore;

namespace LivestockTrading.Application.RequestHandlers.PaymentMethods.Queries.Detail;

public class DataAccess : IDataAccess
{
	private readonly LivestockTradingModuleDbContext _dbContext;

	public DataAccess(ArfBlocksDependencyProvider dbContextProvider)
	{
		_dbContext = dbContextProvider.GetInstance<LivestockTradingModuleDbContext>();
	}

	public async Task<PaymentMethod> GetById(Guid id, CancellationToken ct)
	{
		return await _dbContext.PaymentMethods
			.AsNoTracking()
			.FirstOrDefaultAsync(p => p.Id == id && !p.IsDeleted, ct);
	}
}
