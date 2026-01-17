namespace BaseModules.IAM.Application.RequestHandlers.Roles.Commands.Create;

public class DataAccess : IDataAccess
{
	private readonly IamDbContext _dbContext;

	public DataAccess(ArfBlocksDependencyProvider dependencyProvider)
	{
		_dbContext = dependencyProvider.GetInstance<IamDbContext>();
	}

	public async Task AddRole(Role role)
	{
		_dbContext.AppRoles.Add(role);
		await _dbContext.SaveChangesAsync();
	}
}