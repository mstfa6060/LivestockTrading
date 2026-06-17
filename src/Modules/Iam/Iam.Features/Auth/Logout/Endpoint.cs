using FastEndpoints;
using Iam.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Iam.Features.Auth.Logout;

public sealed class LogoutEndpoint(IamDbContext db) : Endpoint<LogoutRequest, LogoutResponse>
{
    public override void Configure()
    {
        Post("/iam/Auth/Logout");
        AllowAnonymous();
        Tags("Auth");
    }

    public override async Task HandleAsync(LogoutRequest req, CancellationToken ct)
    {
        var token = await db.RefreshTokens
            .FirstOrDefaultAsync(rt => rt.Token == req.RefreshToken, ct);

        if (token is not null)
        {
            db.RefreshTokens.Remove(token);
            await db.SaveChangesAsync(ct);
        }

        await SendAsync(new LogoutResponse(true), 200, ct);
    }
}
