using FastEndpoints;
using Iam.Domain.Entities;
using Iam.Domain.Errors;
using Iam.Features.Services;
using Iam.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Iam.Features.Auth.RefreshToken;

public sealed class RefreshTokenEndpoint(
    IamDbContext db,
    IJwtService jwtService) : Endpoint<RefreshTokenRequest, RefreshTokenResponse>
{
    private static readonly int RefreshTokenDays = 30;

    public override void Configure()
    {
        Post("/Auth/RefreshToken");
        AllowAnonymous();
        Tags("Auth");
    }

    public override async Task HandleAsync(RefreshTokenRequest req, CancellationToken ct)
    {
        var existing = await db.RefreshTokens
            .Include(rt => rt.User)
                .ThenInclude(u => u.UserRoles)
                    .ThenInclude(ur => ur.Role)
            .Include(rt => rt.User)
                .ThenInclude(u => u.UserRoles)
                    .ThenInclude(ur => ur.Module)
            .FirstOrDefaultAsync(rt => rt.Token == req.Token, ct);

        if (existing is null || existing.ExpiresAt < DateTime.UtcNow)
        {
            AddError(IamErrors.Auth.InvalidRefreshToken);
            await SendErrorsAsync(401, ct);
            return;
        }

        if (!existing.User.IsActive)
        {
            AddError(IamErrors.Auth.UserNotFound);
            await SendErrorsAsync(401, ct);
            return;
        }

        // Rotate refresh token
        db.RefreshTokens.Remove(existing);

        var newRefreshToken = new AppRefreshToken
        {
            UserId = existing.UserId,
            Token = jwtService.GenerateRefreshToken(),
            ExpiresAt = DateTime.UtcNow.AddDays(RefreshTokenDays),
            Platform = existing.Platform,
            IpAddress = existing.IpAddress,
        };

        db.RefreshTokens.Add(newRefreshToken);
        await db.SaveChangesAsync(ct);

        var roles = existing.User.UserRoles.Select(ur => $"{ur.Module.Name}.{ur.Role.Name}").ToList();
        var accessToken = jwtService.GenerateAccessToken(existing.User, roles, existing.Platform, newRefreshToken.Id);

        await SendAsync(new RefreshTokenResponse(
            accessToken,
            newRefreshToken.Token,
            newRefreshToken.ExpiresAt
        ), 200, ct);
    }
}
