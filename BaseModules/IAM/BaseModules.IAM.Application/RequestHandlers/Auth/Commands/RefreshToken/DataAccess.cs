using Microsoft.EntityFrameworkCore;

namespace BaseModules.IAM.Application.RequestHandlers.Auth.Commands.RefreshToken;

public class DataAccess : IDataAccess
{
    private readonly IamDbContext _dbContext;

    public DataAccess(ArfBlocksDependencyProvider dependencyProvider)
    {
        _dbContext = dependencyProvider.GetInstance<IamDbContext>();
    }

    public async Task<RefreshTokenUserResult> GetUserByRefreshToken(string refreshToken)
    {
        var token = await _dbContext.AppRefreshTokens
            .FirstOrDefaultAsync(x => x.Token == refreshToken);

        var user = await _dbContext.AppUsers.FirstAsync(x => x.Id == token.UserId);

        return new RefreshTokenUserResult
        {
            User = user,
            RefreshTokenId = token.Id
        };
    }

    public async Task DeleteRefreshTokenById(Guid refreshTokenId)
    {
        var token = await _dbContext.AppRefreshTokens.FirstOrDefaultAsync(x => x.Id == refreshTokenId);

        _dbContext.AppRefreshTokens.Remove(token);
    }

    public Guid AddRefreshToken(Common.Definitions.Domain.Entities.User user, string refreshToken, DateTime expiresAt, string platform, string ip)
    {
        var refreshTokenEntry = new AppRefreshToken
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            Token = refreshToken,
            ExpiresAt = expiresAt,
            UserPlatform = platform,
            IpAddress = ip
        };

        _dbContext.AppRefreshTokens.Add(refreshTokenEntry);
        return refreshTokenEntry.Id;
    }

    public async Task SaveChangesAsync()
    {
        await _dbContext.SaveChangesAsync();
    }

    // 🆕 ADIM 9: Kullanıcının rollerini ModuleId ile birlikte getir
    public async Task<List<UserRoleWithModuleDto>> GetUserRolesWithModule(Guid userId, Guid companyId)
    {
        return await _dbContext.UserRoles
            .Where(ur => ur.UserId == userId && ur.CompanyId == companyId && !ur.IsDeleted)
            .Include(ur => ur.Role)      // Role bilgisini include et
            .Include(ur => ur.Module)    // Module bilgisini include et
            .Select(ur => new UserRoleWithModuleDto
            {
                RoleId = ur.RoleId,
                RoleName = ur.Role.Name,
                ModuleId = ur.ModuleId,
                ModuleName = ur.Module.Name
            })
            .ToListAsync();
    }
}

public class RefreshTokenUserResult
{
    public Common.Definitions.Domain.Entities.User User { get; set; }
    public Guid RefreshTokenId { get; set; }
}

// 🆕 DTO: Rol bilgisi + Modül bilgisi (Login DataAccess'deki ile aynı)
public class UserRoleWithModuleDto
{
    public Guid RoleId { get; set; }
    public string RoleName { get; set; }
    public Guid ModuleId { get; set; }
    public string ModuleName { get; set; }
}