using LivestockTrading.Domain.Entities;
using LivestockTrading.Infrastructure.RelationalDB;
using Microsoft.EntityFrameworkCore;

namespace LivestockTrading.Application.Services;

/// <summary>
/// Abonelik plan limitleri kontrolü (ilan limiti, fotoğraf limiti, boost kredisi)
/// </summary>
public class SubscriptionEnforcementService
{
    private readonly LivestockTradingModuleDbContext _dbContext;

    public SubscriptionEnforcementService(LivestockTradingModuleDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    /// <summary>
    /// Satıcının yeni ilan oluşturup oluşturamayacağını kontrol eder
    /// </summary>
    public async Task<bool> CanCreateListing(Guid sellerId, CancellationToken ct = default)
    {
        var plan = await GetCurrentPlan(sellerId, ct);
        if (plan == null)
        {
            // Free plan varsayılan: 3 ilan
            var activeCount = await GetActiveListingCount(sellerId, ct);
            return activeCount < 3;
        }

        if (plan.MaxActiveListings == 0) // 0 = sınırsız
            return true;

        var count = await GetActiveListingCount(sellerId, ct);
        return count < plan.MaxActiveListings;
    }

    /// <summary>
    /// Satıcının aktif ilan sayısını döndürür
    /// </summary>
    public async Task<int> GetActiveListingCount(Guid sellerId, CancellationToken ct = default)
    {
        return await _dbContext.Products
            .AsNoTracking()
            .CountAsync(p => p.SellerId == sellerId
                && !p.IsDeleted
                && (p.Status == ProductStatus.Active
                    || p.Status == ProductStatus.PendingApproval
                    || p.Status == ProductStatus.Draft), ct);
    }

    /// <summary>
    /// Satıcının mevcut abonelik planını döndürür (null = free)
    /// </summary>
    public async Task<SubscriptionPlan> GetCurrentPlan(Guid sellerId, CancellationToken ct = default)
    {
        var subscription = await _dbContext.SellerSubscriptions
            .AsNoTracking()
            .Include(s => s.SubscriptionPlan)
            .FirstOrDefaultAsync(s => s.SellerId == sellerId
                && s.Status == SubscriptionStatus.Active
                && s.ExpiresAt > DateTime.UtcNow
                && !s.IsDeleted, ct);

        return subscription?.SubscriptionPlan;
    }

    /// <summary>
    /// Satıcının kalan ilan hakkını döndürür (-1 = sınırsız)
    /// </summary>
    public async Task<int> GetRemainingListings(Guid sellerId, CancellationToken ct = default)
    {
        var plan = await GetCurrentPlan(sellerId, ct);
        var maxListings = plan?.MaxActiveListings ?? 3; // Free = 3

        if (maxListings == 0) return -1; // sınırsız

        var activeCount = await GetActiveListingCount(sellerId, ct);
        return Math.Max(0, maxListings - activeCount);
    }

    /// <summary>
    /// Satıcının ilan başına maksimum fotoğraf sayısını döndürür
    /// </summary>
    public async Task<int> GetMaxPhotosPerListing(Guid sellerId, CancellationToken ct = default)
    {
        var plan = await GetCurrentPlan(sellerId, ct);
        return plan?.MaxPhotosPerListing ?? 3; // Free = 3
    }
}
