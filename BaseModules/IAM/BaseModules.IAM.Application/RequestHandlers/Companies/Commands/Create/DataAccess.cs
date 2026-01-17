namespace BaseModules.IAM.Application.RequestHandlers.Companies.Commands.Create;

public class DataAccess : IDataAccess
{
    private readonly IamDbContext _dbContext;

    public DataAccess(ArfBlocksDependencyProvider dependencyProvider)
    {
        _dbContext = dependencyProvider.GetInstance<IamDbContext>();
    }

    public async Task AddCompany(Company company)
    {
        await _dbContext.AppCompanies.AddAsync(company);
        await _dbContext.SaveChangesAsync();
    }
}
