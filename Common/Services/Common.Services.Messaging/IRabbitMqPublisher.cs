namespace Common.Services.Messaging;

public interface IRabbitMqPublisher
{
    Task PublishFanout(string exchangeName, object message);
}

