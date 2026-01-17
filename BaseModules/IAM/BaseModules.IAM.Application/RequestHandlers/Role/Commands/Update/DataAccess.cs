namespace BaseModules.IAM.Application.RequestHandlers.Roles.Commands.Update;

public class DataAccess : IDataAccess
{
	private readonly IamDbContext _dbContext;

	public DataAccess(ArfBlocksDependencyProvider dependencyProvider)
	{
		_dbContext = dependencyProvider.GetInstance<IamDbContext>();
	}

	public async Task UpdateRole(Role role)
	{
		_dbContext.AppRoles.Update(role);
		await _dbContext.SaveChangesAsync();
	}

	public async Task<Role?> GetRoleById(Guid roleId)
	{
		return await _dbContext.AppRoles.FindAsync(roleId);
	}
}
