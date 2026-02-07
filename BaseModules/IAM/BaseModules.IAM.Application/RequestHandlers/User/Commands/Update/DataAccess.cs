using Common.Definitions.Domain.Entities;

namespace BaseModules.IAM.Application.RequestHandlers.Users.Commands.Update;

public class DataAccess : IDataAccess
{
	private readonly IamDbContext _dbContext;

	public DataAccess(ArfBlocksDependencyProvider dependencyProvider)
	{
		_dbContext = dependencyProvider.GetInstance<IamDbContext>();
	}

	public async Task<User> GetById(Guid userId)
	{
		return await _dbContext.AppUsers
			.FirstOrDefaultAsync(u => u.Id == userId && !u.IsDeleted);
	}

	public async Task<User> GetByIdWithCountry(Guid userId)
	{
		return await _dbContext.AppUsers
			.Include(u => u.Country)
			.FirstOrDefaultAsync(u => u.Id == userId && !u.IsDeleted);
	}

	public async Task SaveChanges()
	{
		await _dbContext.SaveChangesAsync();
	}
}
