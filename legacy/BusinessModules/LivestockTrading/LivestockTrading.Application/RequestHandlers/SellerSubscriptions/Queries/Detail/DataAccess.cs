using LivestockTrading.Domain.Entities;
using LivestockTrading.Infrastructure.RelationalDB;
using Microsoft.EntityFrameworkCore;

namespace LivestockTrading.Application.RequestHandlers.SellerSubscriptions.Queries.Detail;

public class DataAccess : IDataAccess
{
	private readonly LivestockTradingModuleDbContext _dbContext;

	public DataAccess(ArfBlocksDependencyProvider dbContextProvider)
	{
		_dbContext = dbContextProvider.GetInstance<LivestockTradingModuleDbContext>();
	}

	public async Task<SellerSubscription> GetBySellerId(Guid sellerId, CancellationToken ct)
	{
		return await _dbContext.SellerSubscriptions
			.AsNoTracking()
			.Include(s => s.SubscriptionPlan)
			.Where(s => s.SellerId == sellerId && !s.IsDeleted && s.Status == SubscriptionStatus.Active)
			.OrderByDescending(s => s.CreatedAt)
			.FirstOrDefaultAsync(ct);
	}

	public async Task<int> GetActiveListingCount(Guid sellerId, CancellationToken ct)
	{
		return await _dbContext.Products
			.AsNoTracking()
			.CountAsync(p => p.SellerId == sellerId
				&& !p.IsDeleted
				&& (p.Status == ProductStatus.Active
					|| p.Status == ProductStatus.PendingApproval
					|| p.Status == ProductStatus.Draft), ct);
	}
}
