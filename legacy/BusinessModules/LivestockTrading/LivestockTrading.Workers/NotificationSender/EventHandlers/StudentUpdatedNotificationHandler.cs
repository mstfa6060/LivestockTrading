using LivestockTrading.Domain.Events;
using LivestockTrading.Workers.NotificationSender.Services;

namespace LivestockTrading.Workers.NotificationSender.EventHandlers;

public class StudentUpdatedNotificationHandler
{
    private readonly IPushNotificationService _pushNotificationService;
    private readonly ILogger<StudentUpdatedNotificationHandler> _logger;

    public StudentUpdatedNotificationHandler(
        IPushNotificationService pushNotificationService,
        ILogger<StudentUpdatedNotificationHandler> logger)
    {
        _pushNotificationService = pushNotificationService;
        _logger = logger;
    }

    public Task HandleAsync(StudentUpdatedEvent studentEvent)
    {
        _logger.LogInformation("Processing StudentUpdatedEvent push notification for student: {StudentId} - {FirstName} {LastName}",
            studentEvent.StudentId, studentEvent.FirstName, studentEvent.LastName);

        var title = "Profile Updated";
        var body = $"Hello {studentEvent.FirstName}, your profile has been updated.";

        var data = new Dictionary<string, string>
        {
            { "type", "student_updated" },
            { "studentId", studentEvent.StudentId.ToString() },
            { "studentNumber", studentEvent.StudentNumber ?? "" },
            { "updatedAt", studentEvent.UpdatedAt.ToString("o") }
        };

        _logger.LogInformation("Push notification prepared - Title: {Title}, Body: {Body}, Data: {Data}",
            title, body, string.Join(", ", data.Select(kv => $"{kv.Key}={kv.Value}")));

        // Example: If you have a device token
        // return _pushNotificationService.SendPushAsync(deviceToken, title, body, data);
        return Task.CompletedTask;
    }
}
