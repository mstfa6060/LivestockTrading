namespace Iam.Features.Services;

public interface INotificationPublisher
{
    Task PublishOtpAsync(string phoneNumber, string otpCode, CancellationToken ct = default);
    Task PublishEmailOtpAsync(string email, string otpCode, CancellationToken ct = default);
    Task PublishPasswordResetAsync(string email, string resetToken, CancellationToken ct = default);
}
