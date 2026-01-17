namespace BaseModules.IAM.Application.RequestHandlers.Companies.Commands.Update;

public class DataAccess : IDataAccess
{
    private readonly IamDbContext _dbContext;

    public DataAccess(ArfBlocksDependencyProvider dependencyProvider)
    {
        _dbContext = dependencyProvider.GetInstance<IamDbContext>();
    }

    public async Task<Company> GetCompanyById(Guid id)
    {
        return await _dbContext.AppCompanies.FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task UpdateCompany(Company company)
    {
        _dbContext.AppCompanies.Update(company);
        await _dbContext.SaveChangesAsync();
    }
}
