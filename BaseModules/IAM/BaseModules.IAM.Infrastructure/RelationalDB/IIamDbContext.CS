namespace BaseModules.IAM.Infrastructure.RelationalDB;

public interface IIamDbContext
{
    DbSet<AppRefreshToken> AppRefreshTokens { get; set; }
    
}