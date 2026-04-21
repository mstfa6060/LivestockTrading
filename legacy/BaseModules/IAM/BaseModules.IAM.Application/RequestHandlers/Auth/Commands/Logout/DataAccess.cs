namespace BaseModules.IAM.Application.RequestHandlers.Auth.Commands.Logout;

public class DataAccess : IDataAccess
{
    private readonly IamDbContext _dbContext;

    public DataAccess(ArfBlocksDependencyProvider dependencyProvider)
    {
        _dbContext = dependencyProvider.GetInstance<IamDbContext>();
    }

    public async Task DeleteRefreshTokenById(Guid refreshTokenId)
    {
        var token = await _dbContext.AppRefreshTokens
            .FirstOrDefaultAsync(x => x.Id == refreshTokenId);

        if (token != null)
        {
            _dbContext.AppRefreshTokens.Remove(token);
            await _dbContext.SaveChangesAsync();
        }
    }
}
