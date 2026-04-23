using LivestockTrading.Domain.Entities;
using LivestockTrading.Infrastructure.RelationalDB;
using Microsoft.EntityFrameworkCore;

namespace LivestockTrading.Application.RequestHandlers.ProductBoosts.Commands.Create;

public class DataAccess : IDataAccess
{
	private readonly LivestockTradingModuleDbContext _dbContext;

	public DataAccess(ArfBlocksDependencyProvider dependencyProvider)
	{
		_dbContext = dependencyProvider.GetInstance<LivestockTradingModuleDbContext>();
	}

	public async Task<BoostPackage> GetBoostPackage(Guid id, CancellationToken ct)
	{
		return await _dbContext.BoostPackages
			.AsNoTracking()
			.FirstOrDefaultAsync(b => b.Id == id && !b.IsDeleted && b.IsActive, ct);
	}

	public async Task Add(ProductBoost entity)
	{
		_dbContext.ProductBoosts.Add(entity);
		await _dbContext.SaveChangesAsync();
	}

	public async Task UpdateProductBoostFields(Guid productId, int boostScore, DateTime expiresAt)
	{
		var product = await _dbContext.Products.FirstOrDefaultAsync(p => p.Id == productId && !p.IsDeleted);
		if (product != null)
		{
			product.IsFeatured = true;
			product.FeaturedUntil = expiresAt;
			product.BoostScore = boostScore;
			await _dbContext.SaveChangesAsync();
		}
	}
}
