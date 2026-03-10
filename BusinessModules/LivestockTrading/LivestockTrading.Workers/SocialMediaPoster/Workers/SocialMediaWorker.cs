using System.Text;
using System.Text.Json;
using LivestockTrading.Domain.Events;
using LivestockTrading.Workers.SocialMediaPoster.EventHandlers;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace LivestockTrading.Workers.SocialMediaPoster.Workers;

public class SocialMediaWorker : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<SocialMediaWorker> _logger;
    private readonly IConfiguration _configuration;
    private IConnection? _connection;
    private IModel? _channel;

    private const string ExchangeName = "livestocktrading.socialmedia.post";
    private const string QueueName = "livestocktrading.socialmedia.post.queue";

    public SocialMediaWorker(
        IServiceProvider serviceProvider,
        ILogger<SocialMediaWorker> logger,
        IConfiguration configuration)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
        _configuration = configuration;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("SocialMediaWorker starting...");

        // Wait a bit for RabbitMQ to be ready
        await Task.Delay(5000, stoppingToken);

        try
        {
            InitializeRabbitMQ();
            await ConsumeMessages(stoppingToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "SocialMediaWorker encountered an error");
        }
    }

    private void InitializeRabbitMQ()
    {
        var host = _configuration["RabbitMq:Host"] ?? "rabbitmq";
        var port = int.Parse(_configuration["RabbitMq:Port"] ?? "5672");
        var user = _configuration["RabbitMq:User"] ?? "guest";
        var pass = _configuration["RabbitMq:Pass"] ?? _configuration["RabbitMq:Password"] ?? "guest";

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

        // Process one message at a time (social media rate limiting)
        _channel.BasicQos(prefetchSize: 0, prefetchCount: 1, global: false);

        _logger.LogInformation("RabbitMQ connection established. Exchange: {Exchange}, Queue: {Queue}", ExchangeName, QueueName);
    }

    private Task ConsumeMessages(CancellationToken stoppingToken)
    {
        var consumer = new EventingBasicConsumer(_channel);

        consumer.Received += async (model, ea) =>
        {
            var body = ea.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);

            _logger.LogInformation("Received social media event: {Message}",
                message.Length > 200 ? message.Substring(0, 200) + "..." : message);

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
                    _logger.LogWarning("Unknown event type in social media message");
                    _channel?.BasicAck(ea.DeliveryTag, false);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing social media message");
                // Don't requeue - social media failures shouldn't block the queue
                _channel?.BasicAck(ea.DeliveryTag, false);
            }
        };

        _channel?.BasicConsume(QueueName, autoAck: false, consumer: consumer);

        return Task.CompletedTask;
    }

    private async Task<bool> TryHandleEvents(string message, IServiceScope scope)
    {
        // Try ProductApprovedEvent
        try
        {
            var productApprovedEvent = JsonSerializer.Deserialize<ProductApprovedEvent>(message);
            if (productApprovedEvent != null && productApprovedEvent.ProductId != Guid.Empty)
            {
                _logger.LogInformation("Handling ProductApprovedEvent for product: {ProductId} - {Title}",
                    productApprovedEvent.ProductId, productApprovedEvent.Title);
                var handler = scope.ServiceProvider.GetRequiredService<ProductApprovedSocialMediaHandler>();
                await handler.HandleAsync(productApprovedEvent);
                return true;
            }
        }
        catch (JsonException ex)
        {
            _logger.LogWarning(ex, "Failed to deserialize as ProductApprovedEvent");
        }

        return false;
    }

    public override void Dispose()
    {
        _channel?.Close();
        _connection?.Close();
        base.Dispose();
    }
}
