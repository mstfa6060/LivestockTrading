using System.Security.Cryptography;
using FastEndpoints;
using Iam.Features.Services;
using Iam.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Iam.Features.Auth.SendOtp;

public sealed class SendOtpEndpoint(
    IamDbContext db,
    INotificationPublisher notificationPublisher) : Endpoint<SendOtpRequest, SendOtpResponse>
{
    public override void Configure()
    {
        Post("/Auth/SendOtp");
        AllowAnonymous();
        Tags("Auth");
    }

    public override async Task HandleAsync(SendOtpRequest req, CancellationToken ct)
    {
        var user = await db.Users.FirstOrDefaultAsync(u => u.PhoneNumber == req.PhoneNumber, ct);
        if (user is null)
        {
            // Don't reveal whether the number exists
            await SendAsync(new SendOtpResponse(true, "OTP kodu gönderildi."), 200, ct);
            return;
        }

        var otp = GenerateOtp();
        user.LastOtpCode = otp;
        user.LastOtpSentAt = DateTime.UtcNow;
        user.LastOtpVerifiedAt = null;

        await db.SaveChangesAsync(ct);
        await notificationPublisher.PublishOtpAsync(req.PhoneNumber, otp, ct);

        await SendAsync(new SendOtpResponse(true, "OTP kodu gönderildi."), 200, ct);
    }

    private static string GenerateOtp()
    {
        var number = RandomNumberGenerator.GetInt32(0, 1_000_000);
        return number.ToString("D6");
    }
}
