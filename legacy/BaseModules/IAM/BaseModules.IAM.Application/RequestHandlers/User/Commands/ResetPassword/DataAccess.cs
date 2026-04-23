namespace BaseModules.IAM.Application.RequestHandlers.Users.Commands.ResetPassword;

public class DataAccess : IDataAccess
{
	private readonly IamDbContext _dbContext;

	public DataAccess(ArfBlocksDependencyProvider dependencyProvider)
	{
		_dbContext = dependencyProvider.GetInstance<IamDbContext>();
	}

	public async Task<Common.Definitions.Domain.Entities.User> GetUserByToken(string token)
	{
		return await _dbContext.AppUsers.FirstOrDefaultAsync(x =>
			x.PasswordResetToken == token && x.PasswordResetTokenExpiry > DateTime.UtcNow);
	}

	public async Task UpdateUser(Common.Definitions.Domain.Entities.User user)
	{
		_dbContext.AppUsers.Update(user);
		await _dbContext.SaveChangesAsync();
	}
}
