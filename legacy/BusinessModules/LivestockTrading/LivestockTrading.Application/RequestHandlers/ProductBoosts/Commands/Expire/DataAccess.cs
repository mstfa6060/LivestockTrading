using LivestockTrading.Domain.Entities;
using LivestockTrading.Infrastructure.RelationalDB;
using Microsoft.EntityFrameworkCore;

namespace LivestockTrading.Application.RequestHandlers.ProductBoosts.Commands.Expire;

public class DataAccess : IDataAccess
{
    private readonly LivestockTradingModuleDbContext _dbContext;

    public DataAccess(ArfBlocksDependencyProvider dependencyProvider)
    {
        _dbContext = dependencyProvider.GetInstance<LivestockTradingModuleDbContext>();
    }

    public async Task<List<ProductBoost>> GetExpiredActiveBoosts(CancellationToken ct)
    {
        return await _dbContext.ProductBoosts
            .Where(b => b.IsActive && !b.IsDeleted && b.ExpiresAt <= DateTime.UtcNow)
            .ToListAsync(ct);
    }

    public async Task<int> ExpireBoosts(List<ProductBoost> expiredBoosts, CancellationToken ct)
    {
        var productIds = expiredBoosts.Select(b => b.ProductId).Distinct().ToList();

        foreach (var boost in expiredBoosts)
            boost.IsActive = false;

        // Her ürün için hâlâ aktif boost var mı kontrol et; yoksa ürün alanlarını sıfırla
        var productsWithActiveBoost = await _dbContext.ProductBoosts
            .Where(b => productIds.Contains(b.ProductId) && b.IsActive && !b.IsDeleted && b.ExpiresAt > DateTime.UtcNow)
            .Select(b => b.ProductId)
            .Distinct()
            .ToListAsync(ct);

        var productsToReset = productIds.Except(productsWithActiveBoost).ToList();
        if (productsToReset.Count > 0)
        {
            var products = await _dbContext.Products
                .Where(p => productsToReset.Contains(p.Id) && !p.IsDeleted)
                .ToListAsync(ct);

            foreach (var product in products)
            {
                product.IsFeatured = false;
                product.FeaturedUntil = null;
                product.BoostScore = 0;
            }
        }

        await _dbContext.SaveChangesAsync(ct);
        return expiredBoosts.Count;
    }
}
