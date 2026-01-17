using Microsoft.Extensions.Hosting;
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

    public EmailWorker(ForgotPasswordEmailHandler handler)
    {
        _handler = handler;
    }

    public override async Task StartAsync(CancellationToken cancellationToken)
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


        _channel.ExchangeDeclare("iam.notification.email", ExchangeType.Fanout, durable: true);
        _channel.QueueDeclare("iam.notification.email.queue", durable: true, exclusive: false, autoDelete: false);
        _channel.QueueBind("iam.notification.email.queue", "iam.notification.email", "");



        Console.WriteLine("[IAM EmailWorker] 🔄 Başlatıldı ve RabbitMQ'ya bağlandı.");

        // Bu satır kritik!
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

                var forgotEvent = JsonSerializer.Deserialize<ForgotPasswordEvent>(message);
                if (forgotEvent is not null)
                {
                    Console.WriteLine($"[IAM EmailWorker] 📥 Mesaj alındı: {forgotEvent.Email} / {forgotEvent.Token}");
                    await _handler.HandleAsync(forgotEvent);
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
