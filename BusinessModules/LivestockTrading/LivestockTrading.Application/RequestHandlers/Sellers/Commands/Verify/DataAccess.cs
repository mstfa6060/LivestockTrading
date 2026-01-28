using LivestockTrading.Domain.Entities;
using LivestockTrading.Infrastructure.RelationalDB;
using Microsoft.EntityFrameworkCore;

namespace LivestockTrading.Application.RequestHandlers.Sellers.Commands.Verify;

public class DataAccess : IDataAccess
{
	private readonly LivestockTradingModuleDbContext _dbContext;

	public DataAccess(ArfBlocksDependencyProvider dependencyProvider)
	{
		_dbContext = dependencyProvider.GetInstance<LivestockTradingModuleDbContext>();
	}

	public async Task<Seller> GetSellerById(Guid sellerId, CancellationToken cancellationToken)
	{
		return await _dbContext.Sellers
			.FirstOrDefaultAsync(s => s.Id == sellerId && !s.IsDeleted, cancellationToken);
	}

	public async Task SaveChanges(CancellationToken cancellationToken)
	{
		await _dbContext.SaveChangesAsync(cancellationToken);
	}
}
