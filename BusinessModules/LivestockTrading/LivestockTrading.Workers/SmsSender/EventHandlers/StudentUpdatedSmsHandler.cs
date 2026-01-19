using LivestockTrading.Domain.Events;
using LivestockTrading.Workers.SmsSender.Services;

namespace LivestockTrading.Workers.SmsSender.EventHandlers;

public class StudentUpdatedSmsHandler
{
    private readonly ISmsService _smsService;
    private readonly ILogger<StudentUpdatedSmsHandler> _logger;

    public StudentUpdatedSmsHandler(
        ISmsService smsService,
        ILogger<StudentUpdatedSmsHandler> logger)
    {
        _smsService = smsService;
        _logger = logger;
    }

    public async Task HandleAsync(StudentUpdatedEvent studentEvent)
    {
        _logger.LogInformation("Processing StudentUpdatedEvent SMS for student: {StudentId} - {FirstName} {LastName}",
            studentEvent.StudentId, studentEvent.FirstName, studentEvent.LastName);

        if (string.IsNullOrEmpty(studentEvent.PhoneNumber))
        {
            _logger.LogWarning("Student {StudentId} has no phone number, skipping SMS notification", studentEvent.StudentId);
            return;
        }

        var message = $"Hi {studentEvent.FirstName}, your GlobalLivestock profile has been updated.";

        await _smsService.SendAsync(studentEvent.PhoneNumber, message);
        _logger.LogInformation("Profile update SMS sent to student {StudentId} at {PhoneNumber}", studentEvent.StudentId, studentEvent.PhoneNumber);
    }
}
