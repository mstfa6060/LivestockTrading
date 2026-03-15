using LivestockTrading.Domain.Entities;
using LivestockTrading.Infrastructure.RelationalDB;
using Microsoft.EntityFrameworkCore;

namespace LivestockTrading.Application.RequestHandlers.SellerSubscriptions.Commands.Create;

public class DataAccess : IDataAccess
{
	private readonly LivestockTradingModuleDbContext _dbContext;

	public DataAccess(ArfBlocksDependencyProvider dependencyProvider)
	{
		_dbContext = dependencyProvider.GetInstance<LivestockTradingModuleDbContext>();
	}

	public async Task Add(SellerSubscription entity)
	{
		_dbContext.SellerSubscriptions.Add(entity);
		await _dbContext.SaveChangesAsync();
	}

	public async Task UpdateSellerActiveSubscription(Guid sellerId, Guid subscriptionId)
	{
		var seller = await _dbContext.Sellers.FirstOrDefaultAsync(s => s.Id == sellerId && !s.IsDeleted);
		if (seller != null)
		{
			seller.ActiveSubscriptionId = subscriptionId;
			await _dbContext.SaveChangesAsync();
		}
	}
}
