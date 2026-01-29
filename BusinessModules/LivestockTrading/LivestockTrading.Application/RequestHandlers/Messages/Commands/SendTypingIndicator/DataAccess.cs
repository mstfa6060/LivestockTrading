using LivestockTrading.Infrastructure.RelationalDB;

namespace LivestockTrading.Application.RequestHandlers.Messages.Commands.SendTypingIndicator;

public class DataAccess : IDataAccess
{
	private readonly LivestockTradingModuleDbContext _dbContext;

	public DataAccess(ArfBlocksDependencyProvider dependencyProvider)
	{
		_dbContext = dependencyProvider.GetInstance<LivestockTradingModuleDbContext>();
	}

	public async Task<bool> ConversationExists(Guid conversationId, CancellationToken cancellationToken)
	{
		return await _dbContext.Conversations
			.AsNoTracking()
			.AnyAsync(c => c.Id == conversationId && !c.IsDeleted, cancellationToken);
	}

	public async Task<bool> IsUserParticipant(Guid conversationId, Guid userId, CancellationToken cancellationToken)
	{
		return await _dbContext.Conversations
			.AsNoTracking()
			.AnyAsync(c => c.Id == conversationId
				&& !c.IsDeleted
				&& (c.ParticipantUserId1 == userId || c.ParticipantUserId2 == userId),
				cancellationToken);
	}
}
