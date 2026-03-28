using Common.Definitions.Domain.Entities;
using LivestockTrading.Domain.Entities;
using LivestockTrading.Infrastructure.RelationalDB;
using Microsoft.EntityFrameworkCore;

namespace LivestockTrading.Application.RequestHandlers.Locations.Commands.Create;

public class DataAccess : IDataAccess
{
	private readonly LivestockTradingModuleDbContext _dbContext;

	public DataAccess(ArfBlocksDependencyProvider dependencyProvider)
	{
		_dbContext = dependencyProvider.GetInstance<LivestockTradingModuleDbContext>();
	}

	public async Task AddLocation(Location location)
	{
		_dbContext.Locations.Add(location);
		await _dbContext.SaveChangesAsync();
	}

	public async Task<District> GetDistrictById(int districtId, CancellationToken ct)
	{
		return await _dbContext.Districts
			.AsNoTracking()
			.FirstOrDefaultAsync(d => d.Id == districtId, ct);
	}
}
