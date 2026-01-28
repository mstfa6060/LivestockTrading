using Arfware.ArfBlocks.Core;
using Arfware.ArfBlocks.Core.Exceptions;
using Common.Services.Auth.CurrentUser;
using LivestockTrading.Application.Constants;
using LivestockTrading.Domain.Enums;
using LivestockTrading.Domain.Errors;
using Common.Services.ErrorCodeGenerator;

namespace LivestockTrading.Application.Authorization;

/// <summary>
/// LivestockTrading modülü için izin kontrolü servisi
/// JWT'deki roller üzerinden Permission kontrolü yapar
/// </summary>
public class PermissionService
{
    private readonly CurrentUserService _currentUserService;

    public PermissionService(ArfBlocksDependencyProvider dependencyProvider)
    {
        _currentUserService = dependencyProvider.GetInstance<CurrentUserService>();
    }

    /// <summary>
    /// Mevcut kullanıcının LivestockTrading modülündeki rollerini getirir
    /// </summary>
    public List<string> GetCurrentUserRoles()
    {
        return _currentUserService.GetUserRolesForModule(LivestockTradingConstants.ModuleName);
    }

    /// <summary>
    /// Mevcut kullanıcının belirtilen role sahip olup olmadığını kontrol eder
    /// </summary>
    public bool HasRole(string roleName)
    {
        return _currentUserService.HasRoleInModule(LivestockTradingConstants.ModuleName, roleName);
    }

    /// <summary>
    /// Mevcut kullanıcının belirtilen rollerden herhangi birine sahip olup olmadığını kontrol eder
    /// </summary>
    public bool HasAnyRole(params string[] roleNames)
    {
        var userRoles = GetCurrentUserRoles();
        return roleNames.Any(r => userRoles.Contains(r));
    }

    /// <summary>
    /// Mevcut kullanıcının belirtilen tüm rollere sahip olup olmadığını kontrol eder
    /// </summary>
    public bool HasAllRoles(params string[] roleNames)
    {
        var userRoles = GetCurrentUserRoles();
        return roleNames.All(r => userRoles.Contains(r));
    }

    /// <summary>
    /// Mevcut kullanıcının belirtilen izne sahip olup olmadığını kontrol eder
    /// </summary>
    public bool HasPermission(Permission permission)
    {
        var userRoles = GetCurrentUserRoles();
        return RolePermissions.HasAnyPermission(userRoles, permission);
    }

    /// <summary>
    /// Mevcut kullanıcının belirtilen izinlerden herhangi birine sahip olup olmadığını kontrol eder
    /// </summary>
    public bool HasAnyPermission(params Permission[] permissions)
    {
        var userRoles = GetCurrentUserRoles();
        return permissions.Any(p => RolePermissions.HasAnyPermission(userRoles, p));
    }

    /// <summary>
    /// Mevcut kullanıcının belirtilen tüm izinlere sahip olup olmadığını kontrol eder
    /// </summary>
    public bool HasAllPermissions(params Permission[] permissions)
    {
        var userRoles = GetCurrentUserRoles();
        return RolePermissions.HasAllPermissions(userRoles, permissions);
    }

    /// <summary>
    /// Mevcut kullanıcının tüm izinlerini getirir
    /// </summary>
    public Permission[] GetCurrentUserPermissions()
    {
        var userRoles = GetCurrentUserRoles();
        return RolePermissions.GetCombinedPermissions(userRoles);
    }

    /// <summary>
    /// Mevcut kullanıcının Admin olup olmadığını kontrol eder
    /// </summary>
    public bool IsAdmin()
    {
        return HasRole(LivestockTradingConstants.Roles.Admin);
    }

    /// <summary>
    /// Mevcut kullanıcının Moderator veya Admin olup olmadığını kontrol eder
    /// </summary>
    public bool IsModerator()
    {
        return HasAnyRole(LivestockTradingConstants.Roles.Admin, LivestockTradingConstants.Roles.Moderator);
    }

    /// <summary>
    /// Mevcut kullanıcının Staff (Admin, Moderator, Support) olup olmadığını kontrol eder
    /// </summary>
    public bool IsStaff()
    {
        return HasAnyRole(
            LivestockTradingConstants.Roles.Admin,
            LivestockTradingConstants.Roles.Moderator,
            LivestockTradingConstants.Roles.Support);
    }

    /// <summary>
    /// Mevcut kullanıcının Seller olup olmadığını kontrol eder
    /// </summary>
    public bool IsSeller()
    {
        return HasRole(LivestockTradingConstants.Roles.Seller);
    }

    /// <summary>
    /// Mevcut kullanıcının Transporter olup olmadığını kontrol eder
    /// </summary>
    public bool IsTransporter()
    {
        return HasRole(LivestockTradingConstants.Roles.Transporter);
    }

    /// <summary>
    /// Mevcut kullanıcının Veterinarian olup olmadığını kontrol eder
    /// </summary>
    public bool IsVeterinarian()
    {
        return HasRole(LivestockTradingConstants.Roles.Veterinarian);
    }

    // ============================================
    // Assert Metodları (Exception fırlatır)
    // ============================================

    /// <summary>
    /// Belirtilen role sahip olunmasını zorunlu kılar
    /// </summary>
    public void RequireRole(string roleName)
    {
        if (!HasRole(roleName))
        {
            throw new ArfBlocksVerificationException(
                ErrorCodeGenerator.GetErrorCode(() => LivestockTradingDomainErrors.AuthorizationErrors.InsufficientPermission));
        }
    }

    /// <summary>
    /// Belirtilen rollerden herhangi birine sahip olunmasını zorunlu kılar
    /// </summary>
    public void RequireAnyRole(params string[] roleNames)
    {
        if (!HasAnyRole(roleNames))
        {
            throw new ArfBlocksVerificationException(
                ErrorCodeGenerator.GetErrorCode(() => LivestockTradingDomainErrors.AuthorizationErrors.InsufficientPermission));
        }
    }

    /// <summary>
    /// Belirtilen izne sahip olunmasını zorunlu kılar
    /// </summary>
    public void RequirePermission(Permission permission)
    {
        if (!HasPermission(permission))
        {
            throw new ArfBlocksVerificationException(
                ErrorCodeGenerator.GetErrorCode(() => LivestockTradingDomainErrors.AuthorizationErrors.InsufficientPermission));
        }
    }

    /// <summary>
    /// Belirtilen izinlerden herhangi birine sahip olunmasını zorunlu kılar
    /// </summary>
    public void RequireAnyPermission(params Permission[] permissions)
    {
        if (!HasAnyPermission(permissions))
        {
            throw new ArfBlocksVerificationException(
                ErrorCodeGenerator.GetErrorCode(() => LivestockTradingDomainErrors.AuthorizationErrors.InsufficientPermission));
        }
    }

    /// <summary>
    /// Admin rolüne sahip olunmasını zorunlu kılar
    /// </summary>
    public void RequireAdmin()
    {
        if (!IsAdmin())
        {
            throw new ArfBlocksVerificationException(
                ErrorCodeGenerator.GetErrorCode(() => LivestockTradingDomainErrors.AuthorizationErrors.AdminRequired));
        }
    }

    /// <summary>
    /// Moderator veya Admin rolüne sahip olunmasını zorunlu kılar
    /// </summary>
    public void RequireModerator()
    {
        if (!IsModerator())
        {
            throw new ArfBlocksVerificationException(
                ErrorCodeGenerator.GetErrorCode(() => LivestockTradingDomainErrors.AuthorizationErrors.ModeratorRequired));
        }
    }

    /// <summary>
    /// Staff (Admin, Moderator, Support) rolüne sahip olunmasını zorunlu kılar
    /// </summary>
    public void RequireStaff()
    {
        if (!IsStaff())
        {
            throw new ArfBlocksVerificationException(
                ErrorCodeGenerator.GetErrorCode(() => LivestockTradingDomainErrors.AuthorizationErrors.StaffRequired));
        }
    }

    /// <summary>
    /// Seller rolüne sahip olunmasını zorunlu kılar
    /// </summary>
    public void RequireSeller()
    {
        if (!IsSeller())
        {
            throw new ArfBlocksVerificationException(
                ErrorCodeGenerator.GetErrorCode(() => LivestockTradingDomainErrors.AuthorizationErrors.SellerRequired));
        }
    }

    /// <summary>
    /// Transporter rolüne sahip olunmasını zorunlu kılar
    /// </summary>
    public void RequireTransporter()
    {
        if (!IsTransporter())
        {
            throw new ArfBlocksVerificationException(
                ErrorCodeGenerator.GetErrorCode(() => LivestockTradingDomainErrors.AuthorizationErrors.TransporterRequired));
        }
    }
}
