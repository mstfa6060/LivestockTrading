using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

public class BrevoSmtpEmailService : IEmailService
{
    private readonly string _smtpHost;
    private readonly int _smtpPort;
    private readonly string _smtpUser;
    private readonly string _smtpPass;
    private readonly string _fromEmail;
    private readonly string _fromName;

    public BrevoSmtpEmailService(string smtpHost, int smtpPort, string smtpUser, string smtpPass, string fromEmail, string fromName)
    {
        _smtpHost = smtpHost;
        _smtpPort = smtpPort;
        _smtpUser = smtpUser;
        _smtpPass = smtpPass;
        _fromEmail = fromEmail;
        _fromName = fromName;
    }

    public async Task SendEmailAsync(string to, string subject, string body)
    {
        Console.WriteLine($"📧 Sending email via Brevo SMTP: {to} → {subject}");
        Console.WriteLine($"📧 Email body: {body}");
        Console.WriteLine($"📧 SMTP Host: {_smtpHost}, Port: {_smtpPort}, User: {_smtpUser}, From: {_fromEmail} ({_fromName})");

        using var smtpClient = new SmtpClient(_smtpHost)
        {
            Port = _smtpPort,
            Credentials = new NetworkCredential(_smtpUser, _smtpPass),
            EnableSsl = true
        };

        var mail = new MailMessage
        {
            From = new MailAddress(_fromEmail, _fromName),
            Subject = subject,
            Body = body,
            IsBodyHtml = true
        };

        mail.To.Add(to);

        await smtpClient.SendMailAsync(mail);
    }
}
