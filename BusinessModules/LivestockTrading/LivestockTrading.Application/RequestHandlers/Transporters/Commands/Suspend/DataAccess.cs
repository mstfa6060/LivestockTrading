using LivestockTrading.Domain.Entities;
using LivestockTrading.Infrastructure.RelationalDB;
using Microsoft.EntityFrameworkCore;

namespace LivestockTrading.Application.RequestHandlers.Transporters.Commands.Suspend;

public class DataAccess : IDataAccess
{
	private readonly LivestockTradingModuleDbContext _dbContext;

	public DataAccess(ArfBlocksDependencyProvider dependencyProvider)
	{
		_dbContext = dependencyProvider.GetInstance<LivestockTradingModuleDbContext>();
	}

	public async Task<Transporter> GetTransporterById(Guid transporterId, CancellationToken cancellationToken)
	{
		return await _dbContext.Transporters
			.FirstOrDefaultAsync(t => t.Id == transporterId && !t.IsDeleted, cancellationToken);
	}

	public async Task SaveChanges(CancellationToken cancellationToken)
	{
		await _dbContext.SaveChangesAsync(cancellationToken);
	}
}
