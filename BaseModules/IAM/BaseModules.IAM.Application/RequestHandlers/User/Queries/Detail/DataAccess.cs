namespace BaseModules.IAM.Application.RequestHandlers.Users.Queries.Detail;

public class DataAccess : IDataAccess
{
	private readonly IamDbContext _dbContext;

	public DataAccess(ArfBlocksDependencyProvider dependencyProvider)
	{
		_dbContext = dependencyProvider.GetInstance<IamDbContext>();
	}

	public async Task<Common.Definitions.Domain.Entities.User> GetUser(Guid userId, Guid companyId)
	{
		return await _dbContext.AppUsers
			.FirstOrDefaultAsync(x => x.Id == userId && x.CompanyId == companyId);
	}
}
