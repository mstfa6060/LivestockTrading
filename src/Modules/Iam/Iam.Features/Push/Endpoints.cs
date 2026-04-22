using System.Security.Claims;
using FastEndpoints;
using Iam.Domain.Entities;
using Iam.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Iam.Features.Push;

public sealed class RegisterPushTokenEndpoint(IamDbContext db) : Endpoint<RegisterPushTokenRequest, RegisterPushTokenResponse>
{
    public override void Configure()
    {
        Post("/iam/Push/RegisterToken");
        Tags("Push");
    }

    public override async Task HandleAsync(RegisterPushTokenRequest req, CancellationToken ct)
    {
        var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!Guid.TryParse(userIdStr, out var userId))
        {
            await SendUnauthorizedAsync(ct);
            return;
        }

        var existing = await db.UserPushTokens
            .FirstOrDefaultAsync(t => t.UserId == userId
                                   && t.DeviceId == req.DeviceId
                                   && t.AppName == req.AppName, ct);

        if (existing is null)
        {
            db.UserPushTokens.Add(new UserPushToken
            {
                UserId = userId,
                DeviceId = req.DeviceId,
                AppName = req.AppName,
                Platform = req.Platform,
                PushToken = req.PushToken,
            });
        }
        else
        {
            existing.PushToken = req.PushToken;
            existing.Platform = req.Platform;
            existing.RevokedAt = null;
            existing.UpdatedAt = DateTime.UtcNow;
        }

        await db.SaveChangesAsync(ct);
        await SendAsync(new RegisterPushTokenResponse(true), 200, ct);
    }
}

public sealed class RevokePushTokenEndpoint(IamDbContext db) : Endpoint<RevokePushTokenRequest, RevokePushTokenResponse>
{
    public override void Configure()
    {
        Post("/iam/Push/RevokeToken");
        Tags("Push");
    }

    public override async Task HandleAsync(RevokePushTokenRequest req, CancellationToken ct)
    {
        var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!Guid.TryParse(userIdStr, out var userId))
        {
            await SendUnauthorizedAsync(ct);
            return;
        }

        var tokens = await db.UserPushTokens
            .Where(t => t.UserId == userId && t.DeviceId == req.DeviceId && t.RevokedAt == null)
            .ToListAsync(ct);

        var now = DateTime.UtcNow;
        foreach (var token in tokens)
        {
            token.RevokedAt = now;
            token.UpdatedAt = now;
        }

        if (tokens.Count > 0)
        {
            await db.SaveChangesAsync(ct);
        }

        await SendAsync(new RevokePushTokenResponse(true), 200, ct);
    }
}
