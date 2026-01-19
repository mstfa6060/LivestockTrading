using System.Text;
using System.Text.Json;
using LivestockTrading.Domain.Events;
using LivestockTrading.Workers.SmsSender.EventHandlers;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace LivestockTrading.Workers.SmsSender.Workers;

public class SmsWorker : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<SmsWorker> _logger;
    private readonly IConfiguration _configuration;
    private IConnection? _connection;
    private IModel? _channel;

    private const string ExchangeName = "livestocktrading.notification.sms";
    private const string QueueName = "livestocktrading.notification.sms.queue";

    public SmsWorker(
        IServiceProvider serviceProvider,
        ILogger<SmsWorker> logger,
        IConfiguration configuration)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
        _configuration = configuration;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("SmsWorker starting...");

        try
        {
            InitializeRabbitMQ();
            await ConsumeMessages(stoppingToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "SmsWorker encountered an error");
        }
    }

    private void InitializeRabbitMQ()
    {
        var host = _configuration["RabbitMq:Host"] ?? Environment.GetEnvironmentVariable("RABBITMQ_HOST") ?? "rabbitmq";
        var port = int.Parse(_configuration["RabbitMq:Port"] ?? Environment.GetEnvironmentVariable("RABBITMQ_PORT") ?? "5672");
        var user = _configuration["RabbitMq:User"] ?? Environment.GetEnvironmentVariable("RABBITMQ_USER") ?? "guest";
        var pass = _configuration["RabbitMq:Password"] ?? Environment.GetEnvironmentVariable("RABBITMQ_PASS") ?? "guest";

        _logger.LogInformation("Connecting to RabbitMQ at {Host}:{Port}", host, port);

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

        _channel.ExchangeDeclare(ExchangeName, ExchangeType.Fanout, durable: true);
        _channel.QueueDeclare(QueueName, durable: true, exclusive: false, autoDelete: false);
        _channel.QueueBind(QueueName, ExchangeName, string.Empty);

        _logger.LogInformation("RabbitMQ connection established. Exchange: {Exchange}, Queue: {Queue}", ExchangeName, QueueName);
    }

    private Task ConsumeMessages(CancellationToken stoppingToken)
    {
        var consumer = new EventingBasicConsumer(_channel);

        consumer.Received += async (model, ea) =>
        {
            var body = ea.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);

            _logger.LogInformation("Received message: {Message}", message.Length > 200 ? message.Substring(0, 200) + "..." : message);

            try
            {
                using var scope = _serviceProvider.CreateScope();
                var handled = await TryHandleEvents(message, scope);

                if (handled)
                {
                    _channel?.BasicAck(ea.DeliveryTag, false);
                }
                else
                {
                    _logger.LogWarning("Unknown event type in message: {Message}", message);
                    _channel?.BasicAck(ea.DeliveryTag, false);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing message");
                _channel?.BasicNack(ea.DeliveryTag, false, true);
            }
        };

        _channel?.BasicConsume(QueueName, autoAck: false, consumer: consumer);

        return Task.CompletedTask;
    }

    private async Task<bool> TryHandleEvents(string message, IServiceScope scope)
    {
        // Try StudentCreatedEvent
        try
        {
            var studentCreatedEvent = JsonSerializer.Deserialize<StudentCreatedEvent>(message);
            if (studentCreatedEvent != null && studentCreatedEvent.StudentId != Guid.Empty)
            {
                _logger.LogInformation("Handling StudentCreatedEvent SMS for student: {StudentId}", studentCreatedEvent.StudentId);
                var handler = scope.ServiceProvider.GetRequiredService<StudentCreatedSmsHandler>();
                await handler.HandleAsync(studentCreatedEvent);
                return true;
            }
        }
        catch { }

        // Try StudentUpdatedEvent
        try
        {
            var studentUpdatedEvent = JsonSerializer.Deserialize<StudentUpdatedEvent>(message);
            if (studentUpdatedEvent != null && studentUpdatedEvent.StudentId != Guid.Empty)
            {
                _logger.LogInformation("Handling StudentUpdatedEvent SMS for student: {StudentId}", studentUpdatedEvent.StudentId);
                var handler = scope.ServiceProvider.GetRequiredService<StudentUpdatedSmsHandler>();
                await handler.HandleAsync(studentUpdatedEvent);
                return true;
            }
        }
        catch { }

        // Try StudentDeletedEvent
        try
        {
            var studentDeletedEvent = JsonSerializer.Deserialize<StudentDeletedEvent>(message);
            if (studentDeletedEvent != null && studentDeletedEvent.StudentId != Guid.Empty)
            {
                _logger.LogInformation("Handling StudentDeletedEvent SMS for student: {StudentId}", studentDeletedEvent.StudentId);
                var handler = scope.ServiceProvider.GetRequiredService<StudentDeletedSmsHandler>();
                await handler.HandleAsync(studentDeletedEvent);
                return true;
            }
        }
        catch { }

        return false;
    }

    public override void Dispose()
    {
        _channel?.Close();
        _connection?.Close();
        base.Dispose();
    }
}
