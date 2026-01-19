using LivestockTrading.Domain.Events;
using LivestockTrading.Workers.SmsSender.Services;

namespace LivestockTrading.Workers.SmsSender.EventHandlers;

public class StudentCreatedSmsHandler
{
    private readonly ISmsService _smsService;
    private readonly ILogger<StudentCreatedSmsHandler> _logger;

    public StudentCreatedSmsHandler(
        ISmsService smsService,
        ILogger<StudentCreatedSmsHandler> logger)
    {
        _smsService = smsService;
        _logger = logger;
    }

    public async Task HandleAsync(StudentCreatedEvent studentEvent)
    {
        _logger.LogInformation("Processing StudentCreatedEvent SMS for student: {StudentId} - {FirstName} {LastName}",
            studentEvent.StudentId, studentEvent.FirstName, studentEvent.LastName);

        if (string.IsNullOrEmpty(studentEvent.PhoneNumber))
        {
            _logger.LogWarning("Student {StudentId} has no phone number, skipping SMS notification", studentEvent.StudentId);
            return;
        }

        var message = $"Welcome to GlobalLivestock, {studentEvent.FirstName}! Your account (#{studentEvent.StudentNumber}) has been created.";

        await _smsService.SendAsync(studentEvent.PhoneNumber, message);
        _logger.LogInformation("Welcome SMS sent to student {StudentId} at {PhoneNumber}", studentEvent.StudentId, studentEvent.PhoneNumber);
    }
}
