namespace LivestockTrading.Application.RequestHandlers.Conversations.Commands.StartWithProduct;

public class DataAccess : IDataAccess
{
	private readonly LivestockTradingModuleDbContext _dbContext;

	public DataAccess(ArfBlocksDependencyProvider dependencyProvider)
	{
		_dbContext = dependencyProvider.GetInstance<LivestockTradingModuleDbContext>();
	}

	public async Task<Conversation> FindExistingConversation(Guid productId, Guid buyerUserId, Guid sellerId, CancellationToken ct)
	{
		var seller = await GetSeller(sellerId, ct);
		if (seller == null) return null;

		return await _dbContext.Conversations
			.AsNoTracking()
			.FirstOrDefaultAsync(c =>
				c.ProductId == productId &&
				!c.IsDeleted &&
				((c.ParticipantUserId1 == buyerUserId && c.ParticipantUserId2 == seller.UserId) ||
				 (c.ParticipantUserId1 == seller.UserId && c.ParticipantUserId2 == buyerUserId)),
				ct);
	}

	public async Task<Seller> GetSeller(Guid sellerId, CancellationToken ct)
	{
		return await _dbContext.Sellers
			.AsNoTracking()
			.FirstOrDefaultAsync(s => s.Id == sellerId && !s.IsDeleted, ct);
	}

	public async Task<string> GetProductTitle(Guid productId, CancellationToken ct)
	{
		return await _dbContext.Products
			.AsNoTracking()
			.Where(p => p.Id == productId && !p.IsDeleted)
			.Select(p => p.Title)
			.FirstOrDefaultAsync(ct);
	}

	public async Task AddConversation(Conversation conversation, CancellationToken ct)
	{
		_dbContext.Conversations.Add(conversation);
		await _dbContext.SaveChangesAsync(ct);
	}

	public async Task AddMessage(Message message, CancellationToken ct)
	{
		_dbContext.Messages.Add(message);
		await _dbContext.SaveChangesAsync(ct);
	}
}
