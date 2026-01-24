using LivestockTrading.Domain.Entities;
using LivestockTrading.Infrastructure.RelationalDB;

namespace LivestockTrading.Application.RequestHandlers.ShippingCarriers.Commands.Create;

public class DataAccess : IDataAccess
{
	private readonly LivestockTradingModuleDbContext _dbContext;

	public DataAccess(ArfBlocksDependencyProvider dependencyProvider)
	{
		_dbContext = dependencyProvider.GetInstance<LivestockTradingModuleDbContext>();
	}

	public async Task AddShippingCarrier(ShippingCarrier shippingCarrier)
	{
		_dbContext.ShippingCarriers.Add(shippingCarrier);
		await _dbContext.SaveChangesAsync();
	}
}
