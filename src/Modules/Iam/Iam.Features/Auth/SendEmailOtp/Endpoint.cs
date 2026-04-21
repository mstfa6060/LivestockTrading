using System.Security.Cryptography;
using FastEndpoints;
using Iam.Features.Services;
using Iam.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Iam.Features.Auth.SendEmailOtp;

public sealed class SendEmailOtpEndpoint(
    IamDbContext db,
    INotificationPublisher notificationPublisher) : Endpoint<SendEmailOtpRequest, SendEmailOtpResponse>
{
    public override void Configure()
    {
        Post("/Auth/SendEmailOtp");
        AllowAnonymous();
        Tags("Auth");
    }

    public override async Task HandleAsync(SendEmailOtpRequest req, CancellationToken ct)
    {
        var user = await db.Users.FirstOrDefaultAsync(u => u.Email == req.Email, ct);
        if (user is null)
        {
            await SendAsync(new SendEmailOtpResponse(true, "OTP kodu gönderildi."), 200, ct);
            return;
        }

        var otp = RandomNumberGenerator.GetInt32(0, 1_000_000).ToString("D6");
        user.LastOtpCode = otp;
        user.LastOtpSentAt = DateTime.UtcNow;
        user.LastOtpVerifiedAt = null;

        await db.SaveChangesAsync(ct);
        await notificationPublisher.PublishEmailOtpAsync(req.Email, otp, ct);

        await SendAsync(new SendEmailOtpResponse(true, "OTP kodu gönderildi."), 200, ct);
    }
}
