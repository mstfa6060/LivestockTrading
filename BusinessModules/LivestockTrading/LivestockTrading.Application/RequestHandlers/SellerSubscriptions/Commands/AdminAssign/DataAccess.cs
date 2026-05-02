using LivestockTrading.Domain.Entities;
using LivestockTrading.Infrastructure.RelationalDB;
using Microsoft.EntityFrameworkCore;

namespace LivestockTrading.Application.RequestHandlers.SellerSubscriptions.Commands.AdminAssign;

public class DataAccess : IDataAccess
{
	private readonly LivestockTradingModuleDbContext _dbContext;

	public DataAccess(ArfBlocksDependencyProvider dependencyProvider)
	{
		_dbContext = dependencyProvider.GetInstance<LivestockTradingModuleDbContext>();
	}

	// Yeni admin-atamasini eklemeden once seller'in mevcut Active subscription kayitlari
	// Cancelled'a cekiliyor; aksi halde SubscriptionEnforcementService eski kayittaki plana
	// bakmaya devam edebilir (FirstOrDefault kullaniyor).
	public async Task DeactivateExistingActiveSubscriptions(Guid sellerId, CancellationToken ct)
	{
		var actives = await _dbContext.SellerSubscriptions
			.Where(s => s.SellerId == sellerId && s.Status == SubscriptionStatus.Active && !s.IsDeleted)
			.ToListAsync(ct);

		var now = DateTime.UtcNow;
		foreach (var s in actives)
		{
			s.Status = SubscriptionStatus.Cancelled;
			s.CancelledAt = now;
			s.UpdatedAt = now;
		}
	}

	public async Task Add(SellerSubscription entity, CancellationToken ct)
	{
		await _dbContext.SellerSubscriptions.AddAsync(entity, ct);
	}

	public async Task UpdateSellerActiveSubscription(Guid sellerId, Guid subscriptionId, CancellationToken ct)
	{
		var seller = await _dbContext.Sellers.FirstOrDefaultAsync(s => s.Id == sellerId && !s.IsDeleted, ct);
		if (seller != null)
			seller.ActiveSubscriptionId = subscriptionId;
	}

	public async Task SaveChanges(CancellationToken ct)
	{
		await _dbContext.SaveChangesAsync(ct);
	}
}
