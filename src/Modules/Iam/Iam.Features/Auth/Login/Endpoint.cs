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
    private static readonly int RefreshTokenDays = 30;

    public override void Configure()
    {
        Post("/Auth/Login");
        AllowAnonymous();
        Tags("Auth");
    }

    public override async Task HandleAsync(LoginRequest req, CancellationToken ct)
    {
        var user = await db.Users
            .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
            .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Module)
            .FirstOrDefaultAsync(u => u.Email == req.Email, ct);

        if (user is null || user.PasswordHash is null || user.PasswordSalt is null)
        {
            AddError(IamErrors.Auth.InvalidCredentials);
            await SendErrorsAsync(401, ct);
            return;
        }

        if (!passwordService.VerifyPassword(req.Password, user.PasswordHash, user.PasswordSalt))
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
        var expiresAt = refreshToken.ExpiresAt;

        await SendAsync(new LoginResponse(
            accessToken,
            refreshToken.Token,
            expiresAt,
            user.Id,
            user.Email,
            user.UserName,
            user.FirstName,
            user.Surname
        ), 200, ct);
    }
}
