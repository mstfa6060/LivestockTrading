using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
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
    private readonly IConfiguration _configuration;
    private readonly ILogger<SmsWorker> _logger;

    public SmsWorker(IServiceProvider serviceProvider, IConfiguration configuration, ILogger<SmsWorker> logger)
    {
        _serviceProvider = serviceProvider;
        _configuration = configuration;
        _logger = logger;
    }

    public override Task StartAsync(CancellationToken cancellationToken)
    {
        var host = _configuration["RabbitMq:Host"] ?? Environment.GetEnvironmentVariable("RABBITMQ_HOST") ?? "rabbitmq";
        var port = int.Parse(_configuration["RabbitMq:Port"] ?? Environment.GetEnvironmentVariable("RABBITMQ_PORT") ?? "5672");
        var user = _configuration["RabbitMq:User"] ?? Environment.GetEnvironmentVariable("RABBITMQ_USER") ?? "guest";
        var pass = _configuration["RabbitMq:Password"] ?? _configuration["RabbitMq:Pass"] ?? Environment.GetEnvironmentVariable("RABBITMQ_PASS") ?? "guest";

        _logger.LogInformation("[SmsWorker] Connecting to RabbitMQ at {Host}:{Port} with user {User}", host, port, user);

        var factory = new ConnectionFactory
        {
            HostName = host,
            Port = port,
            UserName = user,
            Password = pass,
            AutomaticRecoveryEnabled = true,
            NetworkRecoveryInterval = TimeSpan.FromSeconds(5)
        };

        _connection = factory.CreateConnection();
        _channel = _connection.CreateModel();

        _channel.ExchangeDeclare("iam.notification.sms", ExchangeType.Fanout, durable: true);
        _channel.QueueDeclare("iam.notification.sms.queue", durable: true, exclusive: false, autoDelete: false);
        _channel.QueueBind("iam.notification.sms.queue", "iam.notification.sms", "");

        _logger.LogInformation("[SmsWorker] Started and connected to RabbitMQ.");
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
