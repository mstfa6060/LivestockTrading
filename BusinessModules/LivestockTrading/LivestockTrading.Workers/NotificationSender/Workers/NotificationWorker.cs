using System.Text;
using System.Text.Json;
using LivestockTrading.Domain.Events;
using LivestockTrading.Workers.NotificationSender.EventHandlers;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace LivestockTrading.Workers.NotificationSender.Workers;

public class NotificationWorker : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<NotificationWorker> _logger;
    private readonly IConfiguration _configuration;
    private IConnection? _connection;
    private IModel? _channel;

    private const string ExchangeName = "livestocktrading.notification.push";
    private const string QueueName = "livestocktrading.notification.push.queue";

    public NotificationWorker(
        IServiceProvider serviceProvider,
        ILogger<NotificationWorker> logger,
        IConfiguration configuration)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
        _configuration = configuration;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("NotificationWorker starting...");

        try
        {
            InitializeRabbitMQ();
            await ConsumeMessages(stoppingToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "NotificationWorker encountered an error");
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
        // Try MessageCreatedEvent (highest priority - messaging)
        try
        {
            var messageCreatedEvent = JsonSerializer.Deserialize<MessageCreatedEvent>(message);
            if (messageCreatedEvent != null && messageCreatedEvent.MessageId != Guid.Empty)
            {
                _logger.LogInformation("Handling MessageCreatedEvent notification for message: {MessageId}", messageCreatedEvent.MessageId);
                var handler = scope.ServiceProvider.GetRequiredService<MessageCreatedNotificationHandler>();
                await handler.HandleAsync(messageCreatedEvent);
                return true;
            }
        }
        catch { }

        // Try MessageReadEvent
        try
        {
            var messageReadEvent = JsonSerializer.Deserialize<MessageReadEvent>(message);
            if (messageReadEvent != null && messageReadEvent.MessageId != Guid.Empty)
            {
                _logger.LogInformation("Handling MessageReadEvent notification for message: {MessageId}", messageReadEvent.MessageId);
                var handler = scope.ServiceProvider.GetRequiredService<MessageReadNotificationHandler>();
                await handler.HandleAsync(messageReadEvent);
                return true;
            }
        }
        catch { }

        // Try ConversationCreatedEvent
        try
        {
            var conversationCreatedEvent = JsonSerializer.Deserialize<ConversationCreatedEvent>(message);
            if (conversationCreatedEvent != null && conversationCreatedEvent.ConversationId != Guid.Empty)
            {
                _logger.LogInformation("Handling ConversationCreatedEvent notification for conversation: {ConversationId}", conversationCreatedEvent.ConversationId);
                var handler = scope.ServiceProvider.GetRequiredService<ConversationCreatedNotificationHandler>();
                await handler.HandleAsync(conversationCreatedEvent);
                return true;
            }
        }
        catch { }

        // Try ProductApprovedEvent (seller notification - check before ProductCreatedEvent since both have ProductId)
        try
        {
            var productApprovedEvent = JsonSerializer.Deserialize<ProductApprovedEvent>(message);
            if (productApprovedEvent != null && productApprovedEvent.ProductId != Guid.Empty
                && productApprovedEvent.SellerId != Guid.Empty && !string.IsNullOrEmpty(productApprovedEvent.Slug))
            {
                _logger.LogInformation("Handling ProductApprovedEvent notification for product: {ProductId}", productApprovedEvent.ProductId);
                var handler = scope.ServiceProvider.GetRequiredService<ProductApprovedNotificationHandler>();
                await handler.HandleAsync(productApprovedEvent);
                return true;
            }
        }
        catch { }

        // Try ProductCreatedEvent (admin notification for new products)
        try
        {
            var productCreatedEvent = JsonSerializer.Deserialize<ProductCreatedEvent>(message);
            if (productCreatedEvent != null && productCreatedEvent.ProductId != Guid.Empty
                && productCreatedEvent.TargetAdminUserIds != null && productCreatedEvent.TargetAdminUserIds.Count > 0)
            {
                _logger.LogInformation("Handling ProductCreatedEvent notification for product: {ProductId}", productCreatedEvent.ProductId);
                var handler = scope.ServiceProvider.GetRequiredService<ProductCreatedNotificationHandler>();
                await handler.HandleAsync(productCreatedEvent);
                return true;
            }
        }
        catch { }

        // Try StudentCreatedEvent
        try
        {
            var studentCreatedEvent = JsonSerializer.Deserialize<StudentCreatedEvent>(message);
            if (studentCreatedEvent != null && studentCreatedEvent.StudentId != Guid.Empty)
            {
                _logger.LogInformation("Handling StudentCreatedEvent notification for student: {StudentId}", studentCreatedEvent.StudentId);
                var handler = scope.ServiceProvider.GetRequiredService<StudentCreatedNotificationHandler>();
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
                _logger.LogInformation("Handling StudentUpdatedEvent notification for student: {StudentId}", studentUpdatedEvent.StudentId);
                var handler = scope.ServiceProvider.GetRequiredService<StudentUpdatedNotificationHandler>();
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
                _logger.LogInformation("Handling StudentDeletedEvent notification for student: {StudentId}", studentDeletedEvent.StudentId);
                var handler = scope.ServiceProvider.GetRequiredService<StudentDeletedNotificationHandler>();
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
