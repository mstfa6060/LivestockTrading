using LivestockTrading.Domain.Events;
using LivestockTrading.Workers.MailSender.Services;

namespace LivestockTrading.Workers.MailSender.EventHandlers;

public class StudentUpdatedEmailHandler
{
    private readonly IEmailService _emailService;
    private readonly ILogger<StudentUpdatedEmailHandler> _logger;

    public StudentUpdatedEmailHandler(
        IEmailService emailService,
        ILogger<StudentUpdatedEmailHandler> logger)
    {
        _emailService = emailService;
        _logger = logger;
    }

    public async Task HandleAsync(StudentUpdatedEvent studentEvent)
    {
        _logger.LogInformation("Processing StudentUpdatedEvent for student: {StudentId} - {FirstName} {LastName}",
            studentEvent.StudentId, studentEvent.FirstName, studentEvent.LastName);

        if (string.IsNullOrEmpty(studentEvent.Email))
        {
            _logger.LogWarning("Student {StudentId} has no email address, skipping email notification", studentEvent.StudentId);
            return;
        }

        var subject = "GlobalLivestock - Your Profile Has Been Updated";
        var body = $@"
<!DOCTYPE html>
<html>
<head>
    <style>
        body {{ font-family: Arial, sans-serif; line-height: 1.6; color: #333; }}
        .container {{ max-width: 600px; margin: 0 auto; padding: 20px; }}
        .header {{ background-color: #2196F3; color: white; padding: 20px; text-align: center; }}
        .content {{ padding: 20px; background-color: #f9f9f9; }}
        .footer {{ padding: 10px; text-align: center; font-size: 12px; color: #666; }}
    </style>
</head>
<body>
    <div class='container'>
        <div class='header'>
            <h1>Profile Updated</h1>
        </div>
        <div class='content'>
            <h2>Hello {studentEvent.FirstName} {studentEvent.LastName},</h2>
            <p>Your student profile has been updated.</p>
            <p><strong>Student Number:</strong> {studentEvent.StudentNumber}</p>
            <p><strong>Department:</strong> {studentEvent.Department}</p>
            <p><strong>Updated At:</strong> {studentEvent.UpdatedAt:yyyy-MM-dd HH:mm:ss} UTC</p>
            <p>If you did not make this change, please contact our support team immediately.</p>
        </div>
        <div class='footer'>
            <p>© {DateTime.UtcNow.Year} GlobalLivestock. All rights reserved.</p>
        </div>
    </div>
</body>
</html>";

        await _emailService.SendEmailAsync(studentEvent.Email, subject, body);
        _logger.LogInformation("Profile update email sent to student {StudentId} at {Email}", studentEvent.StudentId, studentEvent.Email);
    }
}
