// DataAccess.cs
namespace BaseModules.IAM.Application.RequestHandlers.Users.Commands.UpdatePassword;


// DataAccess
public class DataAccess : IDataAccess
{
	private readonly IamDbContext _dbContext;

	public DataAccess(ArfBlocksDependencyProvider dependencyProvider)
	{
		_dbContext = dependencyProvider.GetInstance<IamDbContext>();
	}

	public async Task<Common.Definitions.Domain.Entities.User> GetUserById(Guid userId)
	{
		return await _dbContext.AppUsers.FirstOrDefaultAsync(u => u.Id == userId);
	}

	public async Task UpdateUser(Common.Definitions.Domain.Entities.User user)
	{
		_dbContext.AppUsers.Update(user);
		await _dbContext.SaveChangesAsync();
	}
}
