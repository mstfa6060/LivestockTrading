using Common.Definitions.Domain.Entities;
using LivestockTrading.Domain.Entities;
using LivestockTrading.Infrastructure.RelationalDB;
using Microsoft.EntityFrameworkCore;

namespace LivestockTrading.Application.RequestHandlers.Locations.Commands.Update;

public class DataAccess : IDataAccess
{
	private readonly LivestockTradingModuleDbContext _dbContext;

	public DataAccess(ArfBlocksDependencyProvider dependencyProvider)
	{
		_dbContext = dependencyProvider.GetInstance<LivestockTradingModuleDbContext>();
	}

	public async Task<Location> GetLocationById(Guid locationId)
	{
		return await _dbContext.Locations
			.FirstOrDefaultAsync(l => l.Id == locationId && !l.IsDeleted);
	}

	public async Task<District> GetDistrictById(int districtId, CancellationToken ct)
	{
		return await _dbContext.Districts
			.AsNoTracking()
			.FirstOrDefaultAsync(d => d.Id == districtId, ct);
	}

	public async Task SaveChanges()
	{
		await _dbContext.SaveChangesAsync();
	}
}
