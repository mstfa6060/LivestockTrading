
namespace BaseModules.IAM.Application.RequestHandlers.Users.Queries.All;

public class DataAccess : IDataAccess
{
	private readonly IamDbContext _dbContext;

	public DataAccess(ArfBlocksDependencyProvider dependencyProvider)
	{
		_dbContext = dependencyProvider.GetInstance<IamDbContext>();
	}

	public async Task<(List<Common.Definitions.Domain.Entities.User> users, Dictionary<Guid, List<string>> userRoles, XPageResponse page)> All(XSorting sorting, List<XFilterItem> filters, XPageRequest pageRequest)
	{
		// GUID FILTER FIX: Prevent ToLower() on Guid fields
		if (filters != null && filters.Any())
		{
			var guidFields = new[] { "id" };

			foreach (var filter in filters.Where(f => f.IsUsed))
			{
				var normalizedKey = filter.Key?.ToLower();
				if (guidFields.Contains(normalizedKey))
				{
					if (filter.ConditionType?.ToLower() == "contains" ||
						string.IsNullOrEmpty(filter.ConditionType))
					{
						filter.ConditionType = "equals";
					}
					filter.Type = "guid";
				}
			}
		}

		var query = _dbContext.AppUsers
			.Where(u => !u.IsDeleted)
			.Sort(sorting)
			.Filter(filters);

		if (sorting == null)
			query = query.OrderByDescending(u => u.UserName);

		var page = query.GetPage(pageRequest);
		var list = await query.Paginate(page).ToListAsync();

		// Kullanıcı ID'lerini al
		var userIds = list.Select(u => u.Id).ToList();

		// UserRoles'u ayrı bir sorgu ile çek (ModuleName.RoleName formatında)
		var userRolesData = await _dbContext.UserRoles
			.Where(ur => userIds.Contains(ur.UserId) && !ur.IsDeleted)
			.Include(ur => ur.Role)
			.Include(ur => ur.Module)
			.Select(ur => new
			{
				ur.UserId,
				RoleName = ur.Module.Name + "." + ur.Role.Name
			})
			.ToListAsync();

		// Dictionary'ye dönüştür
		var userRoles = userRolesData
			.GroupBy(x => x.UserId)
			.ToDictionary(
				g => g.Key,
				g => g.Select(x => x.RoleName).ToList()
			);

		return (list, userRoles, page);
	}
}
