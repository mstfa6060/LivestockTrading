using SendGrid;
using SendGrid.Helpers.Mail;
using System.Net;

namespace LivestockTrading.Workers.MailSender.Services;

public class SendGridEmailService : IEmailService
{
    private readonly string _apiKey;
    private readonly string _fromEmail;
    private readonly string _fromName;
    private readonly ILogger<SendGridEmailService> _logger;

    public SendGridEmailService(string apiKey, string fromEmail, string fromName, ILogger<SendGridEmailService> logger)
    {
        _apiKey = apiKey;
        _fromEmail = fromEmail;
        _fromName = fromName;
        _logger = logger;
    }

    public async Task SendEmailAsync(string to, string subject, string body)
    {
        if (string.IsNullOrEmpty(_apiKey))
        {
            _logger.LogWarning("SendGrid API key not configured. Email not sent to {To}", to);
            return;
        }

        try
        {
            var client = new SendGridClient(_apiKey);
            var from = new EmailAddress(_fromEmail, _fromName);
            var toAddress = new EmailAddress(to);
            var msg = MailHelper.CreateSingleEmail(from, toAddress, subject, null, body);

            var response = await client.SendEmailAsync(msg);

            if (response.StatusCode == HttpStatusCode.OK || response.StatusCode == HttpStatusCode.Accepted)
            {
                _logger.LogInformation("Email sent successfully to {To} with subject: {Subject}", to, subject);
            }
            else
            {
                var responseBody = await response.Body.ReadAsStringAsync();
                _logger.LogError("Failed to send email to {To}. Status: {StatusCode}, Response: {Response}",
                    to, response.StatusCode, responseBody);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending email to {To}", to);
            throw;
        }
    }
}
