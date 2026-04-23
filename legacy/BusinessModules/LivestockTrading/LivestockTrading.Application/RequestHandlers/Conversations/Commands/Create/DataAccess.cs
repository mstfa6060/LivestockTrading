using LivestockTrading.Domain.Entities;
using LivestockTrading.Infrastructure.RelationalDB;

namespace LivestockTrading.Application.RequestHandlers.Conversations.Commands.Create;

public class DataAccess : IDataAccess
{
	private readonly LivestockTradingModuleDbContext _dbContext;

	public DataAccess(ArfBlocksDependencyProvider dependencyProvider)
	{
		_dbContext = dependencyProvider.GetInstance<LivestockTradingModuleDbContext>();
	}

	public async Task AddConversation(Conversation conversation)
	{
		_dbContext.Conversations.Add(conversation);
		await _dbContext.SaveChangesAsync();
	}

	public async Task<string> GetProductTitle(Guid? productId, CancellationToken cancellationToken)
	{
		if (!productId.HasValue)
			return null;

		var product = await _dbContext.Products
			.AsNoTracking()
			.Where(p => p.Id == productId.Value && !p.IsDeleted)
			.Select(p => p.Title)
			.FirstOrDefaultAsync(cancellationToken);

		return product;
	}
}
