using LivestockTrading.Domain.Events;
using LivestockTrading.Workers.MailSender.Services;

namespace LivestockTrading.Workers.MailSender.EventHandlers;

public class StudentDeletedEmailHandler
{
    private readonly IEmailService _emailService;
    private readonly ILogger<StudentDeletedEmailHandler> _logger;

    public StudentDeletedEmailHandler(
        IEmailService emailService,
        ILogger<StudentDeletedEmailHandler> logger)
    {
        _emailService = emailService;
        _logger = logger;
    }

    public async Task HandleAsync(StudentDeletedEvent studentEvent)
    {
        _logger.LogInformation("Processing StudentDeletedEvent for student: {StudentId} - {FirstName} {LastName}",
            studentEvent.StudentId, studentEvent.FirstName, studentEvent.LastName);

        if (string.IsNullOrEmpty(studentEvent.Email))
        {
            _logger.LogWarning("Student {StudentId} has no email address, skipping email notification", studentEvent.StudentId);
            return;
        }

        var subject = "GlobalLivestock - Account Deactivated";
        var body = $@"
<!DOCTYPE html>
<html>
<head>
    <style>
        body {{ font-family: Arial, sans-serif; line-height: 1.6; color: #333; }}
        .container {{ max-width: 600px; margin: 0 auto; padding: 20px; }}
        .header {{ background-color: #f44336; color: white; padding: 20px; text-align: center; }}
        .content {{ padding: 20px; background-color: #f9f9f9; }}
        .footer {{ padding: 10px; text-align: center; font-size: 12px; color: #666; }}
    </style>
</head>
<body>
    <div class='container'>
        <div class='header'>
            <h1>Account Deactivated</h1>
        </div>
        <div class='content'>
            <h2>Hello {studentEvent.FirstName} {studentEvent.LastName},</h2>
            <p>Your student account has been deactivated.</p>
            <p><strong>Student Number:</strong> {studentEvent.StudentNumber}</p>
            <p><strong>Deactivated At:</strong> {studentEvent.DeletedAt:yyyy-MM-dd HH:mm:ss} UTC</p>
            <p>If you believe this was done in error, please contact our support team.</p>
        </div>
        <div class='footer'>
            <p>© {DateTime.UtcNow.Year} GlobalLivestock. All rights reserved.</p>
        </div>
    </div>
</body>
</html>";

        await _emailService.SendEmailAsync(studentEvent.Email, subject, body);
        _logger.LogInformation("Account deactivation email sent to student {StudentId} at {Email}", studentEvent.StudentId, studentEvent.Email);
    }
}
