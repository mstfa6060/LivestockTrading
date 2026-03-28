using Common.Definitions.Domain.Entities;
using LivestockTrading.Domain.Entities;
using LivestockTrading.Infrastructure.RelationalDB;
using Microsoft.EntityFrameworkCore;

namespace LivestockTrading.Application.RequestHandlers.Transporters.Commands.Create;

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

	public async Task<Transporter> GetExistingByUserId(Guid userId, CancellationToken ct)
	{
		return await _dbContext.Transporters
			.FirstOrDefaultAsync(t => t.UserId == userId && !t.IsDeleted, ct);
	}

	public async Task AddTransporter(Transporter transporter)
	{
		_dbContext.Transporters.Add(transporter);
		await _dbContext.SaveChangesAsync();
	}

	public async Task<bool> UserHasTransporterRole(Guid userId, Guid moduleId, Guid roleId)
	{
		return await _dbContext.UserRoles
			.AnyAsync(ur => ur.UserId == userId
				&& ur.ModuleId == moduleId
				&& ur.RoleId == roleId
				&& !ur.IsDeleted);
	}

	public async Task AddUserRole(UserRole userRole)
	{
		_dbContext.UserRoles.Add(userRole);
		await _dbContext.SaveChangesAsync();
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
