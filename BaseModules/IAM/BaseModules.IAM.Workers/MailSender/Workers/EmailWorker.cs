using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;

using BaseModules.IAM.Domain.Events;
using BaseModules.IAM.Workers.MailSender.EventHandlers;

namespace BaseModules.IAM.Workers.MailSender.Workers;

public class EmailWorker : BackgroundService
{
    private IConnection _connection;
    private IModel _channel;
    private readonly ForgotPasswordEmailHandler _handler;
    private readonly IEmailService _emailService;
    private readonly IConfiguration _configuration;
    private readonly ILogger<EmailWorker> _logger;

    public EmailWorker(ForgotPasswordEmailHandler handler, IEmailService emailService, IConfiguration configuration, ILogger<EmailWorker> logger)
    {
        _handler = handler;
        _emailService = emailService;
        _configuration = configuration;
        _logger = logger;
    }

    public override async Task StartAsync(CancellationToken cancellationToken)
    {
        var host = _configuration["RabbitMq:Host"] ?? Environment.GetEnvironmentVariable("RABBITMQ_HOST") ?? "rabbitmq";
        var port = int.Parse(_configuration["RabbitMq:Port"] ?? Environment.GetEnvironmentVariable("RABBITMQ_PORT") ?? "5672");
        var user = _configuration["RabbitMq:User"] ?? Environment.GetEnvironmentVariable("RABBITMQ_USER") ?? "guest";
        var pass = _configuration["RabbitMq:Password"] ?? _configuration["RabbitMq:Pass"] ?? Environment.GetEnvironmentVariable("RABBITMQ_PASS") ?? "guest";

        _logger.LogInformation("[EmailWorker] Connecting to RabbitMQ at {Host}:{Port} with user {User}", host, port, user);

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

        _channel.ExchangeDeclare("iam.notification.email", ExchangeType.Fanout, durable: true);
        _channel.QueueDeclare("iam.notification.email.queue", durable: true, exclusive: false, autoDelete: false);
        _channel.QueueBind("iam.notification.email.queue", "iam.notification.email", "");

        _logger.LogInformation("[EmailWorker] Started and connected to RabbitMQ.");

        await base.StartAsync(cancellationToken);
    }


    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var consumer = new EventingBasicConsumer(_channel);

        consumer.Received += async (model, ea) =>
        {
            try
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);

                if (string.IsNullOrWhiteSpace(message))
                {
                    Console.WriteLine("[IAM EmailWorker] ❗ Gelen mesaj boş olduğu için işlenmedi.");
                    _channel.BasicAck(ea.DeliveryTag, false);
                    return;
                }

                // Route by event type
                using var doc = JsonDocument.Parse(message);
                var hasEventType = doc.RootElement.TryGetProperty("EventType", out var eventTypeProp);
                var eventType = hasEventType ? eventTypeProp.GetString() : null;

                if (eventType == "EmailOtp")
                {
                    var email = doc.RootElement.GetProperty("Email").GetString();
                    var subject = doc.RootElement.GetProperty("Subject").GetString();
                    var emailBody = doc.RootElement.GetProperty("Body").GetString();
                    Console.WriteLine($"[IAM EmailWorker] 📥 EmailOtp mesaj alindi: {email}");
                    await _emailService.SendEmailAsync(email!, subject!, emailBody!);
                }
                else
                {
                    var forgotEvent = JsonSerializer.Deserialize<ForgotPasswordEvent>(message);
                    if (forgotEvent is not null)
                    {
                        Console.WriteLine($"[IAM EmailWorker] 📥 Mesaj alındı: {forgotEvent.Email} / {forgotEvent.Token}");
                        await _handler.HandleAsync(forgotEvent);
                    }
                }

                _channel.BasicAck(ea.DeliveryTag, false);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[IAM EmailWorker] ❌ Hata oluştu: {ex.Message}");
                Console.WriteLine($"[IAM EmailWorker] ❌ Hata ayrıntısı: {ex.StackTrace}");
                Console.WriteLine($"[IAM EmailWorker] ❌ İç hata: {ex.InnerException?.Message}");
                Console.WriteLine($"[IAM EmailWorker] ❌ İç hata ayrıntısı: {ex.InnerException?.StackTrace}");

                _channel.BasicAck(ea.DeliveryTag, false);
            }
        };

        //  SADECE BURADA KULLANILMALI
        _channel.BasicConsume(
            queue: "iam.notification.email.queue",
            autoAck: false,
            consumer: consumer
        );

        Console.WriteLine("[IAM EmailWorker] 📡 Kuyruk dinleniyor...");
        return Task.CompletedTask;
    }

    public override void Dispose()
    {
        _channel?.Close();
        _connection?.Close();
        base.Dispose();
    }
}
