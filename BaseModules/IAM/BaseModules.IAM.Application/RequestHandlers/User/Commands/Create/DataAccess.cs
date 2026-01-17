// DataAccess.cs
namespace BaseModules.IAM.Application.RequestHandlers.Users.Commands.Create;

public class DataAccess : IDataAccess
{
	private readonly IamDbContext _dbContext;

	public DataAccess(ArfBlocksDependencyProvider dependencyProvider)
	{
		_dbContext = dependencyProvider.GetInstance<IamDbContext>();
	}

	public async Task AddUser(Common.Definitions.Domain.Entities.User user)
	{
		try
		{
			_dbContext.AppUsers.Add(user);
			await _dbContext.SaveChangesAsync();
		}
		catch (Exception ex)
		{
			var innerMessage = ex.InnerException?.Message ?? ex.Message;
			throw new Exception("Save error: " + innerMessage, ex);
		}
	}

	public async Task<Common.Definitions.Domain.Entities.User> GetUserByEmail(string email)
	{
		return await _dbContext.AppUsers.FirstOrDefaultAsync(u => u.Email == email);
	}
}
