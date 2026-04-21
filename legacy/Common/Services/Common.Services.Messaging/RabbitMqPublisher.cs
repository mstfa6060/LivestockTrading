#nullable enable
using RabbitMQ.Client;
using System.Collections.Concurrent;
using System.Text;
using System.Text.Json;

namespace Common.Services.Messaging;

public sealed class RabbitMqPublisher : IRabbitMqPublisher, IDisposable
{
    private readonly object _sync = new();
    private ConnectionFactory _factory;
    private IConnection? _conn;
    private IModel? _ch;

    // Aynı exchange'i tekrar declare etmemek için cache
    private ConcurrentDictionary<string, byte> _declaredExchanges = new();

    // 🔴 ÖNEMLİ: PascalCase (PropertyNamingPolicy = null)
    private static readonly JsonSerializerOptions _jsonOpts = new()
    {
        PropertyNamingPolicy = null,
        WriteIndented = false
    };

    public RabbitMqPublisher()
    {
        _factory = BuildFactoryFromEnv();
    }

    public async Task PublishAsync(string exchangeName, object message)
    {
        EnsureConnected();

        if (_ch is null || !_ch.IsOpen)
        {
            Console.WriteLine($"⚠️ RabbitMQ Channel yok. Publish atlandı. (Exchange: {exchangeName})");
            return;
        }

        if (_declaredExchanges.TryAdd(exchangeName, 0))
        {
            _ch.ExchangeDeclare(exchange: exchangeName, type: ExchangeType.Fanout, durable: true, autoDelete: false);
        }

        var json = JsonSerializer.Serialize(message, _jsonOpts);
        var body = Encoding.UTF8.GetBytes(json);

        var props = _ch.CreateBasicProperties();
        props.DeliveryMode = 2; // persistent

        lock (_sync) // channel thread-safe değil
        {
            _ch.BasicPublish(exchange: exchangeName, routingKey: "", basicProperties: props, body: body);
        }

        await Task.CompletedTask;
    }

    public Task PublishFanout(string exchangeName, object message)
        => PublishAsync(exchangeName, message);

    private void EnsureConnected()
    {
        if (_conn is { IsOpen: true } && _ch is { IsOpen: true }) return;

        lock (_sync)
        {
            if (!(_conn?.IsOpen ?? false))
            {
                SafeClose();
                try
                {
                    _conn = _factory.CreateConnection();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"⚠️ RabbitMQ bağlantı kurulamadı. Devam. Hata: {ex.Message}");
                    _conn = null; _ch = null;
                    return;
                }
            }

            if (!(_ch?.IsOpen ?? false))
            {
                try
                {
                    _ch = _conn!.CreateModel();
                    _declaredExchanges = new ConcurrentDictionary<string, byte>(); // channel reset
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"⚠️ RabbitMQ channel açılamadı. Devam. Hata: {ex.Message}");
                    _ch = null;
                }
            }
        }
    }

    private static ConnectionFactory BuildFactoryFromEnv()
    {
        string Get(string? a, string? b, string? d)
            => Environment.GetEnvironmentVariable(a ?? "")
            ?? Environment.GetEnvironmentVariable(b ?? "")
            ?? d
            ?? string.Empty;

        var host = Get("RabbitMq__Host", "RABBITMQ_HOST", "rabbitmq");
        var portS = Get("RabbitMq__Port", "RABBITMQ_PORT", "5672");
        var user = Get("RabbitMq__User", "RABBITMQ_USER", "guest");
        var pass = Get("RabbitMq__Pass", "RABBITMQ_PASS", "guest");
        var vhost = Get("RabbitMq__VHost", "RABBITMQ_VHOST", "/");

        if (!int.TryParse(portS, out var port)) port = 5672;

        return new ConnectionFactory
        {
            HostName = host,
            Port = port,
            UserName = user,
            Password = pass,
            VirtualHost = vhost,
            AutomaticRecoveryEnabled = true,
            TopologyRecoveryEnabled = true,
            RequestedHeartbeat = TimeSpan.FromSeconds(30),
            NetworkRecoveryInterval = TimeSpan.FromSeconds(5),
            DispatchConsumersAsync = true
        };
    }

    private void SafeClose()
    {
        try { _ch?.Close(); } catch { }
        try { _conn?.Close(); } catch { }
        _ch?.Dispose();
        _conn?.Dispose();
        _ch = null; _conn = null;
    }

    public void Dispose() => SafeClose();
}
