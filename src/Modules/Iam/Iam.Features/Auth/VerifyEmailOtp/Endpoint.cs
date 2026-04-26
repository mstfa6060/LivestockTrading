using FastEndpoints;
using Iam.Domain.Errors;
using Iam.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Iam.Features.Auth.VerifyEmailOtp;

public sealed class VerifyEmailOtpEndpoint(IamDbContext db) : Endpoint<VerifyEmailOtpRequest, VerifyEmailOtpResponse>
{
    private static readonly TimeSpan OtpExpiry = TimeSpan.FromMinutes(10);

    public override void Configure()
    {
        Post("/iam/Auth/VerifyEmailOtp");
        AllowAnonymous();
        Tags("Auth");
    }

    public override async Task HandleAsync(VerifyEmailOtpRequest req, CancellationToken ct)
    {
        var user = await db.Users.FirstOrDefaultAsync(u => u.Email == req.Email, ct);

        if (user is null || user.LastOtpCode is null || user.LastOtpSentAt is null)
        {
            AddError(IamErrors.Auth.OtpInvalid);
            await SendErrorsAsync(400, ct);
            return;
        }

        if (DateTime.UtcNow - user.LastOtpSentAt.Value > OtpExpiry)
        {
            AddError(IamErrors.Auth.OtpExpired);
            await SendErrorsAsync(400, ct);
            return;
        }

        if (!string.Equals(user.LastOtpCode, req.OtpCode, StringComparison.Ordinal))
        {
            AddError(IamErrors.Auth.OtpInvalid);
            await SendErrorsAsync(400, ct);
            return;
        }

        user.EmailConfirmed = true;
        user.LastOtpVerifiedAt = DateTime.UtcNow;
        user.LastOtpCode = null;

        await db.SaveChangesAsync(ct);

        await SendAsync(new VerifyEmailOtpResponse(true), 200, ct);
    }
}
