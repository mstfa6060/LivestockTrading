namespace BaseModules.IAM.Application.RequestHandlers.UserPermissions.Queries.My;

public class DataAccess : IDataAccess
{
	private readonly IamDbContext _dbContext;

	public DataAccess(ArfBlocksDependencyProvider dependencyProvider)
	{
		_dbContext = dependencyProvider.GetInstance<IamDbContext>();
	}

	public async Task<List<Guid>> GetUserRoleIds(Guid userId)
	{
		return await _dbContext.UserRoles
			.Where(ur => ur.UserId == userId && !ur.IsDeleted)
			.Select(ur => ur.RoleId)
			.ToListAsync();
	}

	public async Task<List<string>> GetRolePermissions(List<Guid> roleIds)
	{
		return await _dbContext.RolePermissions
			.Where(rp => roleIds.Contains(rp.RoleId) && !rp.IsDeleted)
			.Select(rp => rp.Permission)
			.Distinct()
			.ToListAsync();
	}

	public async Task<List<ResourceDto>> GetUserResources(Guid userId)
	{
		// Kullanıcının erişebileceği resource'ları al
		var roleIds = await GetUserRoleIds(userId);

		return await _dbContext.AppRelRoleResources
			.Where(rrr => roleIds.Contains(rrr.RoleId))
			.Include(rrr => rrr.Resource)
			.Select(rrr => new ResourceDto
			{
				Namespace = rrr.Resource.Namespace,
				Name = rrr.Resource.Name,
				Title = rrr.Resource.Title
			})
			.Distinct()
			.ToListAsync();
	}
}

public class ResourceDto
{
	public string Namespace { get; set; }
	public string Name { get; set; }
	public string Title { get; set; }
}