namespace BaseModules.Notification.Application.RequestHandlers.Push.Commands.RevokeToken;

public class DataAccess : IDataAccess
{
	private readonly IamDbContext _dbContext;

	public DataAccess(ArfBlocksDependencyProvider dependencyProvider)
	{
		_dbContext = dependencyProvider.GetInstance<IamDbContext>();
	}

	public async Task RevokeToken(Guid userId, string deviceId)
	{
		var tokens = await _dbContext.AppUserPushTokens
			.Where(x => x.UserId == userId && x.DeviceId == deviceId && x.RevokedAt == null)
			.ToListAsync();

		foreach (var token in tokens)
		{
			token.RevokedAt = DateTime.UtcNow;
		}

		await _dbContext.SaveChangesAsync();
	}
}
