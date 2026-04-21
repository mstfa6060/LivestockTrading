using LivestockTrading.Domain.Entities;
using LivestockTrading.Infrastructure.RelationalDB;

namespace LivestockTrading.Application.RequestHandlers.Dashboard.Queries.Stats;

public class DataAccess : IDataAccess
{
	private readonly LivestockTradingModuleDbContext _dbContext;

	public DataAccess(ArfBlocksDependencyProvider dbContextProvider)
	{
		_dbContext = dbContextProvider.GetInstance<LivestockTradingModuleDbContext>();
	}

	public async Task<Seller> GetSellerByUserId(Guid userId, CancellationToken ct)
	{
		return await _dbContext.Sellers
			.AsNoTracking()
			.FirstOrDefaultAsync(s => s.UserId == userId && !s.IsDeleted, ct);
	}

	public async Task<int> GetTotalListings(Guid sellerId, CancellationToken ct)
	{
		return await _dbContext.Products
			.CountAsync(p => p.SellerId == sellerId && !p.IsDeleted, ct);
	}

	public async Task<int> GetActiveListings(Guid sellerId, CancellationToken ct)
	{
		return await _dbContext.Products
			.CountAsync(p => p.SellerId == sellerId && !p.IsDeleted && p.Status == ProductStatus.Active, ct);
	}

	public async Task<int> GetTotalViews(Guid sellerId, CancellationToken ct)
	{
		return await _dbContext.Products
			.Where(p => p.SellerId == sellerId && !p.IsDeleted)
			.SumAsync(p => p.ViewCount, ct);
	}

	public async Task<int> GetTotalFavorites(Guid sellerId, CancellationToken ct)
	{
		return await _dbContext.Products
			.Where(p => p.SellerId == sellerId && !p.IsDeleted)
			.SumAsync(p => p.FavoriteCount, ct);
	}

	public async Task<int> GetTotalMessages(Guid userId, CancellationToken ct)
	{
		return await _dbContext.Messages
			.CountAsync(m => (m.SenderUserId == userId || m.RecipientUserId == userId) && !m.IsDeleted, ct);
	}

	public async Task<List<Product>> GetRecentProducts(Guid sellerId, int count, CancellationToken ct)
	{
		return await _dbContext.Products
			.AsNoTracking()
			.Where(p => p.SellerId == sellerId && !p.IsDeleted)
			.OrderByDescending(p => p.CreatedAt)
			.Take(count)
			.ToListAsync(ct);
	}

	public async Task<List<Message>> GetRecentMessages(Guid userId, int count, CancellationToken ct)
	{
		return await _dbContext.Messages
			.AsNoTracking()
			.Where(m => m.RecipientUserId == userId && !m.IsDeleted)
			.OrderByDescending(m => m.SentAt)
			.Take(count)
			.ToListAsync(ct);
	}
}
