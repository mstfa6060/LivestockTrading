using System.Security.Claims;
using FastEndpoints;
using Iam.Features.Services;
using Iam.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Iam.Features.Users.ChangePassword;

public sealed class ChangePasswordEndpoint(IamDbContext db, IPasswordService passwordService) : Endpoint<ChangePasswordRequest, ChangePasswordResponse>
{
    public override void Configure()
    {
        Post("/Users/ChangePassword");
        Tags("Users");
    }

    public override async Task HandleAsync(ChangePasswordRequest req, CancellationToken ct)
    {
        var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!Guid.TryParse(userIdStr, out var userId))
        {
            await SendUnauthorizedAsync(ct);
            return;
        }

        if (req.NewPassword != req.ConfirmNewPassword)
        {
            AddError(r => r.ConfirmNewPassword, "Passwords do not match.");
            await SendErrorsAsync(400, ct);
            return;
        }

        var user = await db.Users.FirstOrDefaultAsync(u => u.Id == userId, ct);
        if (user is null)
        {
            await SendNotFoundAsync(ct);
            return;
        }

        if (user.PasswordHash is null || user.PasswordSalt is null ||
            !passwordService.VerifyPassword(req.CurrentPassword, user.PasswordHash, user.PasswordSalt))
        {
            AddError(r => r.CurrentPassword, "Current password is incorrect.");
            await SendErrorsAsync(400, ct);
            return;
        }

        var (hash, salt) = passwordService.HashPassword(req.NewPassword);
        user.PasswordHash = hash;
        user.PasswordSalt = salt;
        user.UpdatedAt = DateTime.UtcNow;

        await db.SaveChangesAsync(ct);

        await SendAsync(new ChangePasswordResponse(true), 200, ct);
    }
}
