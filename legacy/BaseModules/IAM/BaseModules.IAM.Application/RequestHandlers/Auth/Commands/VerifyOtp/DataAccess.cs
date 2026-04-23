namespace BaseModules.IAM.Application.RequestHandlers.Auth.Commands.VerifyOtp;

public class DataAccess : IDataAccess
{
	private readonly IamDbContext _dbContext;

	public DataAccess(ArfBlocksDependencyProvider provider)
	{
		_dbContext = provider.GetInstance<IamDbContext>();
	}

	public async Task<Common.Definitions.Domain.Entities.User> GetUserByPhoneAsync(string phone)
	{
		return await _dbContext.AppUsers
			.Include(u => u.Country)
			.FirstOrDefaultAsync(x => x.PhoneNumber == phone);
	}

	public async Task UpdateUserAsync(Common.Definitions.Domain.Entities.User user)
	{
		_dbContext.AppUsers.Update(user);
		await _dbContext.SaveChangesAsync();
	}
}
