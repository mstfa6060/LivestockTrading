namespace BaseModules.IAM.Application.RequestHandlers.Companies.Queries.Pick;

public class DataAccess : IDataAccess
{
	private readonly IamDbContext _dbContext;

	public DataAccess(ArfBlocksDependencyProvider dependencyProvider)
	{
		_dbContext = dependencyProvider.GetInstance<IamDbContext>();
	}

	public async Task<List<Company>> GetFiltered(RequestModel request)
	{
		var query = _dbContext.AppCompanies
			.Where(x => !x.IsDeleted)
			.AsQueryable();

		if (!string.IsNullOrWhiteSpace(request.Keyword))
			query = query.Where(x => x.Name.Contains(request.Keyword));

		if (request.SelectedIds?.Any() == true)
			query = query.Where(x => !request.SelectedIds.Contains(x.Id));

		if (request.Limit > 0)
			query = query.Take(request.Limit);

		return await query.OrderBy(x => x.Name).ToListAsync();
	}
}
