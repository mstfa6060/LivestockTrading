using LivestockTrading.Domain.Events;
using LivestockTrading.Workers.MailSender.Services;

namespace LivestockTrading.Workers.MailSender.EventHandlers;

public class StudentCreatedEmailHandler
{
    private readonly IEmailService _emailService;
    private readonly ILogger<StudentCreatedEmailHandler> _logger;

    public StudentCreatedEmailHandler(
        IEmailService emailService,
        ILogger<StudentCreatedEmailHandler> logger)
    {
        _emailService = emailService;
        _logger = logger;
    }

    public async Task HandleAsync(StudentCreatedEvent studentEvent)
    {
        _logger.LogInformation("Processing StudentCreatedEvent for student: {StudentId} - {FirstName} {LastName}",
            studentEvent.StudentId, studentEvent.FirstName, studentEvent.LastName);

        if (string.IsNullOrEmpty(studentEvent.Email))
        {
            _logger.LogWarning("Student {StudentId} has no email address, skipping email notification", studentEvent.StudentId);
            return;
        }

        var subject = "Welcome to GlobalLivestock - Account Created";
        var body = $@"
<!DOCTYPE html>
<html>
<head>
    <style>
        body {{ font-family: Arial, sans-serif; line-height: 1.6; color: #333; }}
        .container {{ max-width: 600px; margin: 0 auto; padding: 20px; }}
        .header {{ background-color: #4CAF50; color: white; padding: 20px; text-align: center; }}
        .content {{ padding: 20px; background-color: #f9f9f9; }}
        .footer {{ padding: 10px; text-align: center; font-size: 12px; color: #666; }}
    </style>
</head>
<body>
    <div class='container'>
        <div class='header'>
            <h1>Welcome to GlobalLivestock</h1>
        </div>
        <div class='content'>
            <h2>Hello {studentEvent.FirstName} {studentEvent.LastName},</h2>
            <p>Your student account has been successfully created.</p>
            <p><strong>Student Number:</strong> {studentEvent.StudentNumber}</p>
            <p><strong>Department:</strong> {studentEvent.Department}</p>
            <p>You can now access all the features available to students.</p>
            <p>If you have any questions, please contact our support team.</p>
        </div>
        <div class='footer'>
            <p>© {DateTime.UtcNow.Year} GlobalLivestock. All rights reserved.</p>
        </div>
    </div>
</body>
</html>";

        await _emailService.SendEmailAsync(studentEvent.Email, subject, body);
        _logger.LogInformation("Welcome email sent to student {StudentId} at {Email}", studentEvent.StudentId, studentEvent.Email);
    }
}
