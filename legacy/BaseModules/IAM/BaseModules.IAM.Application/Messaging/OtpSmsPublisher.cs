using Common.Services.Messaging;
using BaseModules.IAM.Domain.Events;

namespace BaseModules.IAM.Application.Messaging;

public class OtpSmsPublisher
{
    private readonly IRabbitMqPublisher _publisher;

    public OtpSmsPublisher(IRabbitMqPublisher publisher)
    {
        _publisher = publisher;
    }

    public async Task PublishAsync(string phoneNumber, string otpCode)
    {
        var evt = new OtpSmsEvent
        {
            PhoneNumber = phoneNumber,
            Message = $"Doğrulama kodunuz: {otpCode}",
            CreatedAt = DateTime.UtcNow
        };

        await _publisher.PublishFanout("iam.notification.sms", evt);
    }
}
