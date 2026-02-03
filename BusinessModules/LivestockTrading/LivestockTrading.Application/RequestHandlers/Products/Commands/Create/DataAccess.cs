using LivestockTrading.Domain.Entities;
using LivestockTrading.Infrastructure.RelationalDB;
using Microsoft.EntityFrameworkCore;

namespace LivestockTrading.Application.RequestHandlers.Products.Commands.Create;

public class DataAccess : IDataAccess
{
	private readonly LivestockTradingModuleDbContext _dbContext;

	public DataAccess(ArfBlocksDependencyProvider dependencyProvider)
	{
		_dbContext = dependencyProvider.GetInstance<LivestockTradingModuleDbContext>();
	}

	public async Task AddProduct(Product product)
	{
		_dbContext.Products.Add(product);
		await _dbContext.SaveChangesAsync();
	}

	public async Task<Seller> GetSellerByUserId(Guid userId, CancellationToken ct)
	{
		return await _dbContext.Sellers
			.FirstOrDefaultAsync(s => s.UserId == userId && !s.IsDeleted, ct);
	}

	public async Task<Seller> CreateSeller(Seller seller, CancellationToken ct)
	{
		_dbContext.Sellers.Add(seller);
		await _dbContext.SaveChangesAsync(ct);
		return seller;
	}
}
