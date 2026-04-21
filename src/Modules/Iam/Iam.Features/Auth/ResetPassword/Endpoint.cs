using FastEndpoints;
using Iam.Domain.Errors;
using Iam.Features.Services;
using Iam.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Iam.Features.Auth.ResetPassword;

public sealed class ResetPasswordEndpoint(
    IamDbContext db,
    IPasswordService passwordService) : Endpoint<ResetPasswordRequest, ResetPasswordResponse>
{
    public override void Configure()
    {
        Post("/Auth/ResetPassword");
        AllowAnonymous();
        Tags("Auth");
    }

    public override async Task HandleAsync(ResetPasswordRequest req, CancellationToken ct)
    {
        var user = await db.Users
            .FirstOrDefaultAsync(u => u.Email == req.Email && u.PasswordResetToken == req.Token, ct);

        if (user is null || user.PasswordResetTokenExpiresAt < DateTime.UtcNow)
        {
            AddError(IamErrors.Auth.ResetTokenInvalid);
            await SendErrorsAsync(400, ct);
            return;
        }

        var (hash, salt) = passwordService.HashPassword(req.NewPassword);
        user.PasswordHash = hash;
        user.PasswordSalt = salt;
        user.PasswordResetToken = null;
        user.PasswordResetTokenExpiresAt = null;

        await db.SaveChangesAsync(ct);

        await SendAsync(new ResetPasswordResponse(true), 200, ct);
    }
}
