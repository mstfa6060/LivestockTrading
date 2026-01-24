using LivestockTrading.Domain.Entities;
using LivestockTrading.Infrastructure.RelationalDB;
using Microsoft.EntityFrameworkCore;

namespace LivestockTrading.Application.RequestHandlers.ShippingCarriers.Queries.Detail;

public class DataAccess : IDataAccess
{
	private readonly LivestockTradingModuleDbContext _dbContext;

	public DataAccess(ArfBlocksDependencyProvider dbContextProvider)
	{
		_dbContext = dbContextProvider.GetInstance<LivestockTradingModuleDbContext>();
	}

	public async Task<ShippingCarrier> GetById(Guid id, CancellationToken ct)
	{
		return await _dbContext.ShippingCarriers
			.AsNoTracking()
			.FirstOrDefaultAsync(s => s.Id == id && !s.IsDeleted, ct);
	}
}
