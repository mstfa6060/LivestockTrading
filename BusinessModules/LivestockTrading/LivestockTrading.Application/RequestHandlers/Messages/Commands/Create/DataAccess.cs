using LivestockTrading.Domain.Entities;
using LivestockTrading.Domain.Errors;
using LivestockTrading.Infrastructure.RelationalDB;
using Common.Services.ErrorCodeGenerator;
using Microsoft.EntityFrameworkCore;

namespace LivestockTrading.Application.RequestHandlers.Messages.Commands.Create;

public class DataAccess : IDataAccess
{
	private readonly LivestockTradingModuleDbContext _dbContext;

	public DataAccess(ArfBlocksDependencyProvider dependencyProvider)
	{
		_dbContext = dependencyProvider.GetInstance<LivestockTradingModuleDbContext>();
	}

	public async Task<Conversation> GetConversationForUpdate(Guid conversationId, CancellationToken ct)
	{
		var conversation = await _dbContext.Conversations
			.FirstOrDefaultAsync(c => c.Id == conversationId && !c.IsDeleted, ct);

		if (conversation == null)
			throw new ArfBlocksValidationException(
				ErrorCodeGenerator.GetErrorCode(() => LivestockTradingDomainErrors.ConversationErrors.ConversationNotFound));

		return conversation;
	}

	public async Task AddMessageAndTouchConversation(Message message, Conversation conversation, CancellationToken ct)
	{
		conversation.LastMessageAt = message.SentAt;
		_dbContext.Messages.Add(message);
		await _dbContext.SaveChangesAsync(ct);
	}
}
