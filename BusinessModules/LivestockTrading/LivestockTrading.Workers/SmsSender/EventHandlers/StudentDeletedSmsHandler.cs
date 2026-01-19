using LivestockTrading.Domain.Events;
using LivestockTrading.Workers.SmsSender.Services;

namespace LivestockTrading.Workers.SmsSender.EventHandlers;

public class StudentDeletedSmsHandler
{
    private readonly ISmsService _smsService;
    private readonly ILogger<StudentDeletedSmsHandler> _logger;

    public StudentDeletedSmsHandler(
        ISmsService smsService,
        ILogger<StudentDeletedSmsHandler> logger)
    {
        _smsService = smsService;
        _logger = logger;
    }

    public async Task HandleAsync(StudentDeletedEvent studentEvent)
    {
        _logger.LogInformation("Processing StudentDeletedEvent SMS for student: {StudentId} - {FirstName} {LastName}",
            studentEvent.StudentId, studentEvent.FirstName, studentEvent.LastName);

        if (string.IsNullOrEmpty(studentEvent.PhoneNumber))
        {
            _logger.LogWarning("Student {StudentId} has no phone number, skipping SMS notification", studentEvent.StudentId);
            return;
        }

        var message = $"Hi {studentEvent.FirstName}, your GlobalLivestock account has been deactivated. Contact support if this was an error.";

        await _smsService.SendAsync(studentEvent.PhoneNumber, message);
        _logger.LogInformation("Account deactivation SMS sent to student {StudentId} at {PhoneNumber}", studentEvent.StudentId, studentEvent.PhoneNumber);
    }
}
