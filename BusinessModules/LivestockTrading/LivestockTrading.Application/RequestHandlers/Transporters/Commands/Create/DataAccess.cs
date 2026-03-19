using Common.Definitions.Domain.Entities;
using LivestockTrading.Domain.Entities;
using LivestockTrading.Infrastructure.RelationalDB;
using Microsoft.EntityFrameworkCore;

namespace LivestockTrading.Application.RequestHandlers.Transporters.Commands.Create;

public class DataAccess : IDataAccess
{
	private readonly LivestockTradingModuleDbContext _dbContext;

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
}
