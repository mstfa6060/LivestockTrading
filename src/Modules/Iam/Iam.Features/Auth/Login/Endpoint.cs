using FastEndpoints;
using Iam.Domain.Entities;
using Iam.Domain.Enums;
using Iam.Domain.Errors;
using Iam.Features.Services;
using Iam.Persistence;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace Iam.Features.Auth.Login;

public sealed class LoginEndpoint(
    IamDbContext db,
    IJwtService jwtService,
    IPasswordService passwordService) : Endpoint<LoginRequest, LoginResponse>
{
    private const int RefreshTokenDays = 30;

    private static readonly Guid LivestockTradingModuleId = new("DFD018C9-FC32-42C4-AEFD-70A5942A295E");
    private static readonly Guid AdminRoleId = new("a1000000-0000-0000-0000-000000000001");
    private static readonly Guid BuyerRoleId = new("a1000000-0000-0000-0000-000000000006");

    private static readonly string[] AdminEmails =
    [
        "nagehanyazici13@gmail.com",
        "m.mustafaocak@gmail.com",
    ];

    public override void Configure()
    {
        Post("/iam/Auth/Login");
        AllowAnonymous();
        Tags("Auth");
    }

    public override async Task HandleAsync(LoginRequest req, CancellationToken ct)
    {
        var provider = (req.Provider ?? "native").ToLowerInvariant();

        var user = provider switch
        {
            "google" => await ResolveOAuthUserAsync(provider, UserSources.Google, req, ct),
            "apple" or "itunes" => await ResolveOAuthUserAsync("apple", UserSources.Apple, req, ct),
            _ => await ResolveNativeUserAsync(req, ct),
        };

        if (user is null)
        {
            AddError(IamErrors.Auth.InvalidCredentials);
            await SendErrorsAsync(401, ct);
            return;
        }

        if (!user.IsActive)
        {
            AddError(IamErrors.Auth.UserNotFound);
            await SendErrorsAsync(401, ct);
            return;
        }

        var platform = (ClientPlatforms)req.Platform;
        var roles = user.UserRoles.Select(ur => $"{ur.Module.Name}.{ur.Role.Name}").ToList();

        var refreshToken = new AppRefreshToken
        {
            UserId = user.Id,
            Token = jwtService.GenerateRefreshToken(),
            ExpiresAt = DateTime.UtcNow.AddDays(RefreshTokenDays),
            Platform = platform,
            IpAddress = HttpContext.Connection.RemoteIpAddress?.ToString(),
        };

        db.RefreshTokens.Add(refreshToken);
        await db.SaveChangesAsync(ct);

        var accessToken = jwtService.GenerateAccessToken(user, roles, platform, refreshToken.Id);

        await SendAsync(new LoginResponse(
            accessToken,
            refreshToken.Token,
            refreshToken.ExpiresAt,
            user.Id,
            user.Email,
            user.UserName,
            user.FirstName,
            user.Surname,
            user.AuthProvider ?? "native"
        ), 200, ct);
    }

    private async Task<User?> ResolveNativeUserAsync(LoginRequest req, CancellationToken ct)
    {
        var identifier = !string.IsNullOrWhiteSpace(req.UserName) ? req.UserName : req.Email;
        if (string.IsNullOrWhiteSpace(identifier) || string.IsNullOrWhiteSpace(req.Password))
        {
            return null;
        }

        var user = await db.Users
            .Include(u => u.UserRoles).ThenInclude(ur => ur.Role)
            .Include(u => u.UserRoles).ThenInclude(ur => ur.Module)
            .FirstOrDefaultAsync(u => u.Email == identifier || u.UserName == identifier, ct);

        if (user is null || user.PasswordHash is null || user.PasswordSalt is null)
        {
            return null;
        }

        if (!passwordService.VerifyPassword(req.Password, user.PasswordHash, user.PasswordSalt))
        {
            return null;
        }

        if (!string.IsNullOrEmpty(req.PhoneNumber) && user.PhoneNumber != req.PhoneNumber)
        {
            user.PhoneNumber = req.PhoneNumber;
            user.UpdatedAt = DateTime.UtcNow;
            await db.SaveChangesAsync(ct);
        }

        return user;
    }

    private async Task<User?> ResolveOAuthUserAsync(
        string authProvider,
        UserSources source,
        LoginRequest req,
        CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(req.ExternalProviderUserId) || string.IsNullOrWhiteSpace(req.Email))
        {
            return null;
        }

        var user = await db.Users
            .Include(u => u.UserRoles).ThenInclude(ur => ur.Role)
            .Include(u => u.UserRoles).ThenInclude(ur => ur.Module)
            .FirstOrDefaultAsync(u => u.AuthProvider == authProvider && u.ProviderKey == req.ExternalProviderUserId, ct);

        if (user is not null)
        {
            ApplyOAuthProfileUpdates(user, req);
            await db.SaveChangesAsync(ct);
            return user;
        }

        var existing = await db.Users
            .Include(u => u.UserRoles).ThenInclude(ur => ur.Role)
            .Include(u => u.UserRoles).ThenInclude(ur => ur.Module)
            .FirstOrDefaultAsync(u => u.Email == req.Email, ct);

        if (existing is not null)
        {
            existing.AuthProvider = authProvider;
            existing.ProviderKey = req.ExternalProviderUserId;
            existing.UserSource = source;
            ApplyOAuthProfileUpdates(existing, req);
            await EnsureBuyerOrAdminRoleAsync(existing, ct);
            await db.SaveChangesAsync(ct);
            return existing;
        }

        var created = new User
        {
            UserName = req.UserName ?? req.Email,
            Email = req.Email,
            FirstName = req.FirstName ?? authProvider,
            Surname = req.Surname ?? "User",
            PhoneNumber = req.PhoneNumber,
            BirthDate = req.BirthDate,
            AuthProvider = authProvider,
            ProviderKey = req.ExternalProviderUserId,
            UserSource = source,
            IsActive = true,
            EmailConfirmed = true,
        };

        db.Users.Add(created);
        await EnsureBuyerOrAdminRoleAsync(created, ct);
        await db.SaveChangesAsync(ct);

        return await db.Users
            .Include(u => u.UserRoles).ThenInclude(ur => ur.Role)
            .Include(u => u.UserRoles).ThenInclude(ur => ur.Module)
            .FirstAsync(u => u.Id == created.Id, ct);
    }

    private static void ApplyOAuthProfileUpdates(User user, LoginRequest req)
    {
        if (!string.IsNullOrWhiteSpace(req.FirstName))
        {
            user.FirstName = req.FirstName;
        }
        if (!string.IsNullOrWhiteSpace(req.Surname))
        {
            user.Surname = req.Surname;
        }
        if (!string.IsNullOrWhiteSpace(req.PhoneNumber))
        {
            user.PhoneNumber = req.PhoneNumber;
        }
        user.UpdatedAt = DateTime.UtcNow;
    }

    private async Task EnsureBuyerOrAdminRoleAsync(User user, CancellationToken ct)
    {
        var hasRole = await db.UserRoles
            .AnyAsync(ur => ur.UserId == user.Id && ur.ModuleId == LivestockTradingModuleId, ct);
        if (hasRole)
        {
            return;
        }

        var roleId = AdminEmails.Contains(user.Email, StringComparer.OrdinalIgnoreCase)
            ? AdminRoleId
            : BuyerRoleId;

        db.UserRoles.Add(new UserRole
        {
            UserId = user.Id,
            RoleId = roleId,
            ModuleId = LivestockTradingModuleId,
        });
    }
}
