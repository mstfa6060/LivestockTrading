namespace BaseModules.IAM.Application.RequestHandlers.User.Commands.Delete;

public class DataAccess : IDataAccess
{
	private readonly IamDbContext _dbContext;

	public DataAccess(ArfBlocksDependencyProvider dependencyProvider)
	{
		_dbContext = dependencyProvider.GetInstance<IamDbContext>();
	}

	public async Task<Common.Definitions.Domain.Entities.User> GetUserById(Guid userId)
	{
		return await _dbContext.AppUsers.FirstOrDefaultAsync(x => x.Id == userId);
	}

	public async Task DeleteUser(Common.Definitions.Domain.Entities.User user)
	{
		_dbContext.AppUsers.Update(user);
		await _dbContext.SaveChangesAsync();
	}
}
