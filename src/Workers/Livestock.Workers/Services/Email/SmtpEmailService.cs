using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Livestock.Workers.Services.Email;

public sealed class SmtpEmailService(IConfiguration config, ILogger<SmtpEmailService> logger) : IEmailService
{
    public async Task SendAsync(string to, string subject, string htmlBody, CancellationToken ct = default)
    {
        var host = config["Smtp:Host"] ?? "smtp-relay.brevo.com";
        var port = int.TryParse(config["Smtp:Port"], out var p) ? p : 587;
        var user = config["Smtp:User"] ?? string.Empty;
        var pass = config["Smtp:Pass"] ?? string.Empty;
        var fromEmail = config["Smtp:FromEmail"] ?? "noreply@globallivestock.com";
        var fromName = config["Smtp:FromName"] ?? "GlobalLivestock";

#pragma warning disable SYSLIB0006
        using var client = new SmtpClient(host, port)
        {
            Credentials = new NetworkCredential(user, pass),
            EnableSsl = true
        };

        using var mail = new MailMessage
        {
            From = new MailAddress(fromEmail, fromName),
            Subject = subject,
            Body = htmlBody,
            IsBodyHtml = true
        };
        mail.To.Add(to);
#pragma warning restore SYSLIB0006

        try
        {
            await client.SendMailAsync(mail, ct);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "SMTP send failed to {To}", to);
        }
    }
}
