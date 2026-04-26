using Microsoft.Extensions.Logging;

namespace Livestock.Workers.Services.Email;

public sealed class LoggingEmailService(ILogger<LoggingEmailService> logger) : IEmailService
{
    public Task SendAsync(string to, string subject, string htmlBody, CancellationToken ct = default)
    {
        logger.LogInformation("[EMAIL] To={To} Subject={Subject}", to, subject);
        return Task.CompletedTask;
    }
}
