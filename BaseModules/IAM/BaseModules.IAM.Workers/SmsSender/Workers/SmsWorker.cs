using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text.Json;
using Common.Contracts.Queue.Models;
using BaseModules.IAM.Workers.SmsSender.EventHandlers;
using System.Text;

namespace BaseModules.IAM.Workers.SmsSender.Workers;

public class SmsWorker : BackgroundService
{
    private IConnection _connection;
    private IModel _channel;
    private readonly IServiceProvider _serviceProvider;

    public SmsWorker(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public override Task StartAsync(CancellationToken cancellationToken)
    {
        var factory = new ConnectionFactory
        {
            HostName = Environment.GetEnvironmentVariable("RABBITMQ_HOST") ?? "rabbitmq",
            Port = int.Parse(Environment.GetEnvironmentVariable("RABBITMQ_PORT") ?? "5672"),
            UserName = Environment.GetEnvironmentVariable("RABBITMQ_USER") ?? "guest",
            Password = Environment.GetEnvironmentVariable("RABBITMQ_PASS") ?? "guest",
            AutomaticRecoveryEnabled = true,
            NetworkRecoveryInterval = TimeSpan.FromSeconds(5)
        };


        _connection = factory.CreateConnection();
        _channel = _connection.CreateModel();

        _channel.ExchangeDeclare("iam.notification.sms", ExchangeType.Fanout, durable: true);
        _channel.QueueDeclare("iam.notification.sms.queue", durable: true, exclusive: false, autoDelete: false);
        _channel.QueueBind("iam.notification.sms.queue", "iam.notification.sms", "");

        Console.WriteLine("[SmsWorker] 🔄 Başlatıldı ve RabbitMQ'ya bağlandı.");
        return base.StartAsync(cancellationToken);
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var consumer = new EventingBasicConsumer(_channel);
        consumer.Received += async (model, ea) =>
        {
            try
            {
                using var scope = _serviceProvider.CreateScope();
                var handler = scope.ServiceProvider.GetRequiredService<OtpSmsHandler>();

                var body = ea.Body.ToArray();
                var json = Encoding.UTF8.GetString(body);
                var smsEvent = JsonSerializer.Deserialize<SmsModelContract>(json);

                if (smsEvent != null)
                    await handler.HandleAsync(smsEvent);

                _channel.BasicAck(ea.DeliveryTag, false);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ [SmsWorker] Hata: {ex.Message}");
                _channel.BasicAck(ea.DeliveryTag, false);
            }
        };

        _channel.BasicConsume("iam.notification.sms.queue", autoAck: false, consumer: consumer);
        return Task.CompletedTask;
    }

    public override void Dispose()
    {
        _channel?.Close();
        _connection?.Close();
        base.Dispose();
    }
}
