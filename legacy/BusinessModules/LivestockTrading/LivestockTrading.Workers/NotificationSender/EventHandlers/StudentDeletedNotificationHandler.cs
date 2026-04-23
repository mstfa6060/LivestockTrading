using LivestockTrading.Domain.Events;
using LivestockTrading.Workers.NotificationSender.Services;

namespace LivestockTrading.Workers.NotificationSender.EventHandlers;

public class StudentDeletedNotificationHandler
{
    private readonly IPushNotificationService _pushNotificationService;
    private readonly ILogger<StudentDeletedNotificationHandler> _logger;

    public StudentDeletedNotificationHandler(
        IPushNotificationService pushNotificationService,
        ILogger<StudentDeletedNotificationHandler> logger)
    {
        _pushNotificationService = pushNotificationService;
        _logger = logger;
    }

    public Task HandleAsync(StudentDeletedEvent studentEvent)
    {
        _logger.LogInformation("Processing StudentDeletedEvent push notification for student: {StudentId} - {FirstName} {LastName}",
            studentEvent.StudentId, studentEvent.FirstName, studentEvent.LastName);

        var title = "Account Deactivated";
        var body = $"Hello {studentEvent.FirstName}, your account has been deactivated.";

        var data = new Dictionary<string, string>
        {
            { "type", "student_deleted" },
            { "studentId", studentEvent.StudentId.ToString() },
            { "studentNumber", studentEvent.StudentNumber ?? "" },
            { "deletedAt", studentEvent.DeletedAt.ToString("o") }
        };

        _logger.LogInformation("Push notification prepared - Title: {Title}, Body: {Body}, Data: {Data}",
            title, body, string.Join(", ", data.Select(kv => $"{kv.Key}={kv.Value}")));

        // Example: If you have a device token
        // return _pushNotificationService.SendPushAsync(deviceToken, title, body, data);
        return Task.CompletedTask;
    }
}
