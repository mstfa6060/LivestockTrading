namespace BaseModules.IAM.Application.RequestHandlers.Auth.Commands.RevokeRefreshToken;

public class DataAccess : IDataAccess
{
    private readonly IamDbContext _dbContext;

    public DataAccess(ArfBlocksDependencyProvider dependencyProvider)
    {
        _dbContext = dependencyProvider.GetInstance<IamDbContext>();
    }

    public async Task RevokeToken(Guid refreshTokenId)
    {
        var token = await _dbContext.AppRefreshTokens.FirstOrDefaultAsync(x => x.Id == refreshTokenId);
        _dbContext.AppRefreshTokens.Remove(token);
        await _dbContext.SaveChangesAsync();
    }
}
