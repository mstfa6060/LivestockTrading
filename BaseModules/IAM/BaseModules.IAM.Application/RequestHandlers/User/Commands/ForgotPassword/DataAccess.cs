namespace BaseModules.IAM.Application.RequestHandlers.Users.Commands.ForgotPassword;

public class DataAccess : IDataAccess
{
	private readonly IamDbContext _dbContext;

	public DataAccess(ArfBlocksDependencyProvider dependencyProvider)
	{
		_dbContext = dependencyProvider.GetInstance<IamDbContext>();
	}

	public async Task<Common.Definitions.Domain.Entities.User> GetUserByEmail(string email, Guid companyId)
	{
		if (string.IsNullOrEmpty(email))
			return null;

		return await _dbContext.AppUsers
			.FirstOrDefaultAsync(x => x.Email == email && x.CompanyId == companyId);
	}

	public async Task UpdateUser(Common.Definitions.Domain.Entities.User user)
	{
		_dbContext.AppUsers.Update(user);
		await _dbContext.SaveChangesAsync();
	}
}
