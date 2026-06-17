using Microsoft.Extensions.Logging;

namespace Iam.Features.Services;

public sealed class LoggingNotificationPublisher(ILogger<LoggingNotificationPublisher> logger) : INotificationPublisher
{
    public Task PublishOtpAsync(string phoneNumber, string otpCode, CancellationToken ct = default)
    {
        logger.LogInformation("OTP {Code} would be sent to {Phone}", otpCode, phoneNumber);
        return Task.CompletedTask;
    }

    public Task PublishEmailOtpAsync(string email, string otpCode, CancellationToken ct = default)
    {
        logger.LogInformation("Email OTP {Code} would be sent to {Email}", otpCode, email);
        return Task.CompletedTask;
    }

    public Task PublishPasswordResetAsync(string email, string resetToken, CancellationToken ct = default)
    {
        logger.LogInformation("Password reset token {Token} would be sent to {Email}", resetToken, email);
        return Task.CompletedTask;
    }
}
