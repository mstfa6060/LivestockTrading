using System.Threading.Tasks;

namespace BaseModules.IAM.Application.RequestHandlers.Auth.Commands.SendOtp;

public class DataAccess : IDataAccess
{
	private readonly IamDbContext _dbContext;

	public DataAccess(ArfBlocksDependencyProvider provider)
	{
		_dbContext = provider.GetInstance<IamDbContext>();
	}

	public async Task<Common.Definitions.Domain.Entities.User> GetUserById(Guid userId)
	{
		return await _dbContext.AppUsers
			.FirstOrDefaultAsync(x => x.Id == userId);
	}

	public void UpdateUserOtp(Common.Definitions.Domain.Entities.User user)
	{
		_dbContext.AppUsers.Update(user);
	}
	public async Task UpdateUser(Common.Definitions.Domain.Entities.User user)
	{
		_dbContext.AppUsers.Update(user);
		await _dbContext.SaveChangesAsync();
	}

	public async Task SaveChanges()
	{
		await _dbContext.SaveChangesAsync();
	}
	public async Task<Guid> AddRefreshToken(Common.Definitions.Domain.Entities.User user, string token, DateTime expiresAt, string platform, string ip)
	{
		var refreshToken = new AppRefreshToken
		{
			Id = Guid.NewGuid(),
			UserId = user.Id,
			Token = token,
			ExpiresAt = expiresAt,
			UserPlatform = platform,
			IpAddress = ip,
			CreatedAt = DateTime.UtcNow
		};

		_dbContext.AppRefreshTokens.Add(refreshToken);
		await _dbContext.SaveChangesAsync();
		return refreshToken.Id;
	}
}
