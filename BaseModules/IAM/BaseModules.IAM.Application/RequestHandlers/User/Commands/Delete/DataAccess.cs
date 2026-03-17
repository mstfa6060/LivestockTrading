namespace BaseModules.IAM.Application.RequestHandlers.Users.Commands.Delete;

public class DataAccess : IDataAccess
{
	private readonly IamDbContext _dbContext;

	public DataAccess(ArfBlocksDependencyProvider dependencyProvider)
	{
		_dbContext = dependencyProvider.GetInstance<IamDbContext>();
	}

	public async Task<Common.Definitions.Domain.Entities.User> GetUserById(Guid userId, CancellationToken ct)
	{
		return await _dbContext.AppUsers.FirstOrDefaultAsync(x => x.Id == userId && !x.IsDeleted, ct);
	}

	public async Task SoftDeleteUser(Common.Definitions.Domain.Entities.User user, CancellationToken ct)
	{
		user.IsDeleted = true;
		user.DeletedAt = DateTime.UtcNow;
		user.IsActive = false;

		_dbContext.AppUsers.Update(user);
		await _dbContext.SaveChangesAsync(ct);
	}

	public async Task DeleteAllRefreshTokens(Guid userId, CancellationToken ct)
	{
		var tokens = await _dbContext.AppRefreshTokens
			.Where(x => x.UserId == userId)
			.ToListAsync(ct);

		if (tokens.Any())
		{
			_dbContext.AppRefreshTokens.RemoveRange(tokens);
			await _dbContext.SaveChangesAsync(ct);
		}
	}

	public async Task SoftDeleteUserRoles(Guid userId, CancellationToken ct)
	{
		var userRoles = await _dbContext.UserRoles
			.Where(x => x.UserId == userId && !x.IsDeleted)
			.ToListAsync(ct);

		foreach (var role in userRoles)
		{
			role.IsDeleted = true;
			role.DeletedAt = DateTime.UtcNow;
		}

		if (userRoles.Any())
		{
			await _dbContext.SaveChangesAsync(ct);
		}
	}

	/// <summary>
	/// Soft-delete related LivestockTrading data via raw SQL.
	/// Since IAM module doesn't have access to LivestockTrading DbContext,
	/// we use raw SQL to update the shared database tables.
	/// </summary>
	public async Task SoftDeleteRelatedLivestockTradingData(Guid userId, CancellationToken ct)
	{
		var now = DateTime.UtcNow;

		// 1. Get seller IDs for this user
		var sellerIds = await _dbContext.Database
			.SqlQueryRaw<Guid>("SELECT Id FROM Sellers WHERE UserId = {0} AND IsDeleted = 0", userId)
			.ToListAsync(ct);

		if (sellerIds.Any())
		{
			// 2. Soft-delete products belonging to the user's seller accounts
			foreach (var sellerId in sellerIds)
			{
				await _dbContext.Database.ExecuteSqlRawAsync(
					"UPDATE Products SET IsDeleted = 1, DeletedAt = {0} WHERE SellerId = {1} AND IsDeleted = 0",
					now, sellerId);
			}

			// 3. Soft-delete seller records
			await _dbContext.Database.ExecuteSqlRawAsync(
				"UPDATE Sellers SET IsDeleted = 1, DeletedAt = {0}, IsActive = 0 WHERE UserId = {1} AND IsDeleted = 0",
				now, userId);
		}

		// 4. Soft-delete conversations where user is a participant
		await _dbContext.Database.ExecuteSqlRawAsync(
			"UPDATE Conversations SET IsDeleted = 1, DeletedAt = {0} WHERE (ParticipantUserId1 = {1} OR ParticipantUserId2 = {1}) AND IsDeleted = 0",
			now, userId);

		// 5. Soft-delete messages sent by the user
		await _dbContext.Database.ExecuteSqlRawAsync(
			"UPDATE Messages SET IsDeleted = 1, DeletedAt = {0} WHERE SenderUserId = {1} AND IsDeleted = 0",
			now, userId);

		// 6. Soft-delete offers where user is buyer or seller
		await _dbContext.Database.ExecuteSqlRawAsync(
			"UPDATE Offers SET IsDeleted = 1, DeletedAt = {0} WHERE (BuyerUserId = {1} OR SellerUserId = {1}) AND IsDeleted = 0",
			now, userId);

		// 7. Soft-delete favorite products
		await _dbContext.Database.ExecuteSqlRawAsync(
			"UPDATE FavoriteProducts SET IsDeleted = 1, DeletedAt = {0} WHERE UserId = {1} AND IsDeleted = 0",
			now, userId);

		// 8. Soft-delete product view histories
		await _dbContext.Database.ExecuteSqlRawAsync(
			"UPDATE ProductViewHistories SET IsDeleted = 1, DeletedAt = {0} WHERE UserId = {1} AND IsDeleted = 0",
			now, userId);

		// 9. Soft-delete product reviews by user
		await _dbContext.Database.ExecuteSqlRawAsync(
			"UPDATE ProductReviews SET IsDeleted = 1, DeletedAt = {0} WHERE UserId = {1} AND IsDeleted = 0",
			now, userId);

		// 10. Soft-delete seller reviews by user
		await _dbContext.Database.ExecuteSqlRawAsync(
			"UPDATE SellerReviews SET IsDeleted = 1, DeletedAt = {0} WHERE UserId = {1} AND IsDeleted = 0",
			now, userId);
	}
}
