namespace BaseModules.Notification.Application.RequestHandlers.Push.Commands.RegisterToken;

public class DataAccess : IDataAccess
{
	private readonly IamDbContext _dbContext;

	public DataAccess(ArfBlocksDependencyProvider dependencyProvider)
	{
		_dbContext = dependencyProvider.GetInstance<IamDbContext>();
	}

	public async Task SavePushToken(Guid userId, string token, string platform, string deviceId, string appName)
	{
		var existing = await _dbContext.AppUserPushTokens
			.FirstOrDefaultAsync(x => x.UserId == userId && x.DeviceId == deviceId && x.AppName == appName);

		if (existing is null)
		{
			var newToken = new UserPushToken
			{
				UserId = userId,
				DeviceId = deviceId,
				AppName = appName,
				Platform = Enum.Parse<UserPushPlatform>(platform),
				PushToken = token,
				CreatedAt = DateTime.UtcNow
			};

			_dbContext.AppUserPushTokens.Add(newToken);
		}
		else
		{
			existing.PushToken = token;
			existing.Platform = Enum.Parse<UserPushPlatform>(platform);
			existing.RevokedAt = null;
		}

		await _dbContext.SaveChangesAsync();
	}
}
