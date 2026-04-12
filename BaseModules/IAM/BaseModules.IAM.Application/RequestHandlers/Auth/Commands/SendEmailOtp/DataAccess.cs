namespace BaseModules.IAM.Application.RequestHandlers.Auth.Commands.SendEmailOtp;

public class DataAccess : IDataAccess
{
    private readonly IamDbContext _dbContext;

    public DataAccess(ArfBlocksDependencyProvider provider)
    {
        _dbContext = provider.GetInstance<IamDbContext>();
    }

    public async Task<Common.Definitions.Domain.Entities.User> GetUserById(Guid userId)
    {
        return await _dbContext.AppUsers.FirstOrDefaultAsync(x => x.Id == userId);
    }

    public async Task<Common.Definitions.Domain.Entities.User> GetUserByEmail(string email)
    {
        return await _dbContext.AppUsers.FirstOrDefaultAsync(x => x.Email == email);
    }

    public async Task SaveChanges()
    {
        await _dbContext.SaveChangesAsync();
    }
}
