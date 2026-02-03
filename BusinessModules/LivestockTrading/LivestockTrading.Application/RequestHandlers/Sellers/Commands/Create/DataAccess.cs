using Common.Definitions.Domain.Entities;
using LivestockTrading.Domain.Entities;
using LivestockTrading.Infrastructure.RelationalDB;
using Microsoft.EntityFrameworkCore;

namespace LivestockTrading.Application.RequestHandlers.Sellers.Commands.Create;

public class DataAccess : IDataAccess
{
	private readonly LivestockTradingModuleDbContext _dbContext;

	public DataAccess(ArfBlocksDependencyProvider dependencyProvider)
	{
		_dbContext = dependencyProvider.GetInstance<LivestockTradingModuleDbContext>();
	}

	public async Task AddSeller(Seller seller)
	{
		_dbContext.Sellers.Add(seller);
		await _dbContext.SaveChangesAsync();
	}

	public async Task<bool> UserHasSellerRole(Guid userId, Guid moduleId, Guid sellerRoleId)
	{
		return await _dbContext.UserRoles
			.AnyAsync(ur => ur.UserId == userId
				&& ur.ModuleId == moduleId
				&& ur.RoleId == sellerRoleId
				&& !ur.IsDeleted);
	}

	public async Task AddUserRole(UserRole userRole)
	{
		_dbContext.UserRoles.Add(userRole);
		await _dbContext.SaveChangesAsync();
	}
}
