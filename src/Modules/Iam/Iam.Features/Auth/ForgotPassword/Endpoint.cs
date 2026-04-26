using System.Security.Cryptography;
using FastEndpoints;
using Iam.Features.Services;
using Iam.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Iam.Features.Auth.ForgotPassword;

public sealed class ForgotPasswordEndpoint(
    IamDbContext db,
    INotificationPublisher notificationPublisher) : Endpoint<ForgotPasswordRequest, ForgotPasswordResponse>
{
    public override void Configure()
    {
        Post("/iam/Auth/ForgotPassword");
        AllowAnonymous();
        Tags("Auth");
    }

    public override async Task HandleAsync(ForgotPasswordRequest req, CancellationToken ct)
    {
        var user = await db.Users.FirstOrDefaultAsync(u => u.Email == req.Email, ct);

        // Always return success to avoid email enumeration
        if (user is null)
        {
            await SendAsync(new ForgotPasswordResponse(true, "Şifre sıfırlama bağlantısı gönderildi."), 200, ct);
            return;
        }

        var token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(48));
        user.PasswordResetToken = token;
        user.PasswordResetTokenExpiresAt = DateTime.UtcNow.AddHours(1);

        await db.SaveChangesAsync(ct);
        await notificationPublisher.PublishPasswordResetAsync(req.Email, token, ct);

        await SendAsync(new ForgotPasswordResponse(true, "Şifre sıfırlama bağlantısı gönderildi."), 200, ct);
    }
}
