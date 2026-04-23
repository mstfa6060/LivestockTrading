using LivestockTrading.Infrastructure.RelationalDB;
using Microsoft.EntityFrameworkCore;

namespace LivestockTrading.Application.RequestHandlers.Products.Queries.MediaDetail;

public class DataAccess : IDataAccess
{
	private readonly LivestockTradingModuleDbContext _dbContext;

	public DataAccess(ArfBlocksDependencyProvider dependencyProvider)
	{
		_dbContext = dependencyProvider.GetInstance<LivestockTradingModuleDbContext>();
	}

	public Task<ResponseModel> GetMediaAsync(Guid productId, CancellationToken ct)
		=> _dbContext.Products
			.AsNoTracking()
			.Where(p => p.Id == productId && !p.IsDeleted)
			.Select(p => new ResponseModel
			{
				ProductId = p.Id,
				MediaBucketId = p.MediaBucketId,
				CoverImageFileId = p.CoverImageFileId
			})
			.FirstOrDefaultAsync(ct);
}
