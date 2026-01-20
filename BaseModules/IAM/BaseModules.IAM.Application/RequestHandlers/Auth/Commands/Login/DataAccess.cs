using BaseModules.IAM.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace BaseModules.IAM.Application.RequestHandlers.Auth.Commands.Login;

public class DataAccess : IDataAccess
{
    private readonly IamDbContext _dbContext;

    public DataAccess(ArfBlocksDependencyProvider dependencyProvider)
    {
        _dbContext = dependencyProvider.GetInstance<IamDbContext>();
    }

    public async Task<Common.Definitions.Domain.Entities.User> GetUserById(Guid userId)
    {
        return await _dbContext.AppUsers.SingleAsync(u => u.Id == userId);
    }

    public async Task<Common.Definitions.Domain.Entities.User> GetUser(string identifier)
    {
        return await _dbContext.AppUsers
            .FirstOrDefaultAsync(u =>
                u.UserName.ToLower() == identifier.ToLower() || u.Email.ToLower() == identifier.ToLower());
    }

    public async Task<Common.Definitions.Domain.Entities.User> GetUserByExternalId(string provider, string externalId)
    {
        return await _dbContext.AppUsers.FirstOrDefaultAsync(u =>
            u.AuthProvider == provider && u.ProviderKey == externalId);
    }

    public void AddUser(Common.Definitions.Domain.Entities.User user)
    {
        _dbContext.AppUsers.Add(user);
    }

    public void UpdateUser(Common.Definitions.Domain.Entities.User user)
    {
        _dbContext.AppUsers.Update(user);
    }

    public async Task<Role> GetRoleById(Guid roleId)
    {
        return await _dbContext.AppRoles.FirstOrDefaultAsync(x => x.Id == roleId);
    }

    public void AddRoleUser(UserRole roleUser)
    {
        _dbContext.UserRoles.Add(roleUser);
    }

    public async Task SaveChanges()
    {
        await _dbContext.SaveChangesAsync();
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

    // Kullanıcının rollerini ModuleId ile birlikte getir
    public async Task<List<UserRoleWithModuleDto>> GetUserRolesWithModule(Guid userId)
    {
        return await _dbContext.UserRoles
            .Where(ur => ur.UserId == userId && !ur.IsDeleted)
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

    /// <summary>
    /// Kullanıcının belirli bir modülde rolü var mı kontrol et
    /// </summary>
    public async Task<bool> HasModuleRole(Guid userId, Guid moduleId)
    {
        return await _dbContext.UserRoles
            .AnyAsync(ur => ur.UserId == userId && ur.ModuleId == moduleId && !ur.IsDeleted);
    }

    /// <summary>
    /// Modül bilgisini ID ile getir
    /// </summary>
    public async Task<Common.Definitions.Domain.Entities.Module> GetModuleById(Guid moduleId)
    {
        return await _dbContext.AppModules.FirstOrDefaultAsync(m => m.Id == moduleId);
    }

    /// <summary>
    /// UserRole kaydı ekle (ModuleId dahil)
    /// </summary>
    public void AddUserRoleWithModule(UserRole userRole)
    {
        _dbContext.UserRoles.Add(userRole);
    }
}

// 🆕 DTO: Rol bilgisi + Modül bilgisi
public class UserRoleWithModuleDto
{
    public Guid RoleId { get; set; }
    public string RoleName { get; set; }
    public Guid ModuleId { get; set; }
    public string ModuleName { get; set; }
}