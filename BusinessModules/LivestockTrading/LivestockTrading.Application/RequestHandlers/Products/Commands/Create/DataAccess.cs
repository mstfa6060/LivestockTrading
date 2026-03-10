using LivestockTrading.Domain.Entities;
using LivestockTrading.Infrastructure.RelationalDB;
using Microsoft.EntityFrameworkCore;

namespace LivestockTrading.Application.RequestHandlers.Products.Commands.Create;

public class DataAccess : IDataAccess
{
	private readonly LivestockTradingModuleDbContext _dbContext;

	private static readonly Guid LivestockTradingModuleId = Guid.Parse("DFD018C9-FC32-42C4-AEFD-70A5942A295E");
	private static readonly Guid AdminRoleId = Guid.Parse("a1000000-0000-0000-0000-000000000001");
	private static readonly Guid ModeratorRoleId = Guid.Parse("a1000000-0000-0000-0000-000000000002");

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

	public async Task<List<Guid>> GetAdminModeratorUserIds(CancellationToken ct)
	{
		return await _dbContext.UserRoles
			.AsNoTracking()
			.Where(ur => !ur.IsDeleted
				&& ur.ModuleId == LivestockTradingModuleId
				&& (ur.RoleId == AdminRoleId || ur.RoleId == ModeratorRoleId))
			.Select(ur => ur.UserId)
			.Distinct()
			.ToListAsync(ct);
	}

	public async Task CreateNotifications(List<Notification> notifications, CancellationToken ct)
	{
		_dbContext.Notifications.AddRange(notifications);
		await _dbContext.SaveChangesAsync(ct);
	}
}
