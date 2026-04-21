using BaseModules.IAM.Domain.Events;
using Common.Services.Messaging;

namespace BaseModules.IAM.Application.Messaging;

public class ForgotPasswordEmailPublisher
{
    private readonly IRabbitMqPublisher _publisher;

    public ForgotPasswordEmailPublisher(IRabbitMqPublisher publisher)
    {
        _publisher = publisher;
    }

    public async Task PublishAsync(ForgotPasswordEvent @event)
    {
        await _publisher.PublishFanout("iam.notification.email", @event);
    }
}
