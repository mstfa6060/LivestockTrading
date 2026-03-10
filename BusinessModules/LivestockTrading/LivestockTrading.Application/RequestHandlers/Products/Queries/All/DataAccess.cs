using LivestockTrading.Domain.Entities;
using LivestockTrading.Infrastructure.RelationalDB;
using Microsoft.EntityFrameworkCore;

namespace LivestockTrading.Application.RequestHandlers.Products.Queries.All;

public class DataAccess : IDataAccess
{
	private readonly LivestockTradingModuleDbContext _dbContext;

	public DataAccess(ArfBlocksDependencyProvider dbContextProvider)
	{
		_dbContext = dbContextProvider.GetInstance<LivestockTradingModuleDbContext>();
	}

	public async Task<(List<Product> Products, XPageResponse Page)> All(
		string countryCode,
		XSorting sorting,
		List<XFilterItem> filters,
		XPageRequest pageRequest,
		CancellationToken ct)
	{
		var query = _dbContext.Products
			.AsNoTracking()
			.Include(p => p.Location)
			.Where(p => !p.IsDeleted);

		// Ülke filtresi - belirtilmişse sadece o ülkenin ürünlerini getir
		if (!string.IsNullOrWhiteSpace(countryCode))
		{
			query = query.Where(p => p.Location != null && p.Location.CountryCode == countryCode);
		}

		query = query.Sort(sorting).Filter(filters);

		// Default sorting
		if (sorting == null)
			query = query.OrderByDescending(p => p.CreatedAt);

		// Pagination
		var page = query.GetPage(pageRequest);
		var products = await query.Paginate(page).ToListAsync(ct);

		return (products, page);
	}

	public async Task<Dictionary<string, string>> GetCoverImagePaths(List<string> fileIds, CancellationToken ct)
	{
		var result = new Dictionary<string, string>();
		var validIds = fileIds.Where(id => !string.IsNullOrWhiteSpace(id) && Guid.TryParse(id, out _)).Distinct().ToList();
		if (validIds.Count == 0)
			return result;

		var connection = _dbContext.Database.GetDbConnection();
		if (connection.State != System.Data.ConnectionState.Open)
			await connection.OpenAsync(ct);

		// Batch query with IN clause
		var paramNames = validIds.Select((_, i) => $"@p{i}").ToList();
		using var cmd = connection.CreateCommand();
		cmd.CommandText = $"SELECT Id, [Path] FROM FileEntries WHERE Id IN ({string.Join(",", paramNames)})";

		for (int i = 0; i < validIds.Count; i++)
		{
			var param = cmd.CreateParameter();
			param.ParameterName = $"@p{i}";
			param.Value = validIds[i];
			cmd.Parameters.Add(param);
		}

		using var reader = await cmd.ExecuteReaderAsync(ct);
		while (await reader.ReadAsync(ct))
		{
			var id = reader.GetString(0);
			var path = reader.IsDBNull(1) ? null : reader.GetString(1);
			if (!string.IsNullOrEmpty(path))
				result[id] = path;
		}

		return result;
	}
}
