namespace BaseModules.IAM.Application.RequestHandlers.Roles.Queries.All;

public class DataAccess : IDataAccess
{
	private readonly IamDbContext _dbContext;

	public DataAccess(ArfBlocksDependencyProvider dependencyProvider)
	{
		_dbContext = dependencyProvider.GetInstance<IamDbContext>();
	}

	public async Task<(List<Role>, XPageResponse)> All(XSorting sorting, List<XFilterItem> filters, XPageRequest pageRequest)
	{
		//  GUID FILTER FIX: Prevent ToLower() on Guid fields
		// Guid fields in Role: Id
		if (filters != null && filters.Any())
		{
			var guidFields = new[] { "id" };

			foreach (var filter in filters.Where(f => f.IsUsed))
			{
				var normalizedKey = filter.Key?.ToLower();
				if (guidFields.Contains(normalizedKey))
				{
					// Force Guid fields to use "equals" or "in" condition types
					// to prevent string operations like "contains" that call ToLower()
					if (filter.ConditionType?.ToLower() == "contains" ||
						string.IsNullOrEmpty(filter.ConditionType))
					{
						filter.ConditionType = "equals";
					}
					filter.Type = "guid";
				}
			}
		}

		var query = _dbContext.AppRoles
			.Sort(sorting)
			.Filter(filters);

		if (sorting == null)
			query = query.OrderByDescending(c => c.Name);

		var page = query.GetPage(pageRequest);
		var list = await query.Paginate(page).ToListAsync();

		return (list, page);
	}
}
