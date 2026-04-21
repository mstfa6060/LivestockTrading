using Microsoft.Extensions.Logging;

namespace Livestock.Workers.Services.Sms;

public sealed class LoggingSmsService(ILogger<LoggingSmsService> logger) : ISmsService
{
    public Task SendAsync(string phoneNumber, string message, CancellationToken ct = default)
    {
        logger.LogInformation("[SMS] To={Phone} Message={Message}", phoneNumber, message);
        return Task.CompletedTask;
    }
}
