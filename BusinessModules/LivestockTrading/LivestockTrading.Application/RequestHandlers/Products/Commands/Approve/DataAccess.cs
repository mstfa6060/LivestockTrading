using LivestockTrading.Domain.Entities;
using LivestockTrading.Infrastructure.RelationalDB;
using Microsoft.EntityFrameworkCore;

namespace LivestockTrading.Application.RequestHandlers.Products.Commands.Approve;

public class DataAccess : IDataAccess
{
	private readonly LivestockTradingModuleDbContext _dbContext;

	public DataAccess(ArfBlocksDependencyProvider dependencyProvider)
	{
		_dbContext = dependencyProvider.GetInstance<LivestockTradingModuleDbContext>();
	}

	public async Task<Product> GetProductById(Guid productId)
	{
		return await _dbContext.Products
			.FirstOrDefaultAsync(p => p.Id == productId && !p.IsDeleted);
	}

	public async Task<Product> GetProductWithDetails(Guid productId)
	{
		return await _dbContext.Products
			.Include(p => p.Category)
			.Include(p => p.Brand)
			.Include(p => p.Seller)
			.Include(p => p.Location)
			.FirstOrDefaultAsync(p => p.Id == productId && !p.IsDeleted);
	}

	public async Task<string> GetCoverImagePath(string coverImageFileId)
	{
		if (string.IsNullOrWhiteSpace(coverImageFileId) || !Guid.TryParse(coverImageFileId, out _))
			return null;

		var connection = _dbContext.Database.GetDbConnection();
		if (connection.State != System.Data.ConnectionState.Open)
			await connection.OpenAsync();

		using var cmd = connection.CreateCommand();
		cmd.CommandText = "SELECT [Path] FROM FileEntries WHERE Id = @Id";
		var param = cmd.CreateParameter();
		param.ParameterName = "@Id";
		param.Value = coverImageFileId;
		cmd.Parameters.Add(param);

		var result = await cmd.ExecuteScalarAsync();
		return result as string;
	}

	public async Task SaveChanges()
	{
		await _dbContext.SaveChangesAsync();
	}
}
