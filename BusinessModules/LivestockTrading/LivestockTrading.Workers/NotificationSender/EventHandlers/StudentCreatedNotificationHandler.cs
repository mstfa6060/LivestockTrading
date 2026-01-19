using LivestockTrading.Domain.Events;
using LivestockTrading.Workers.NotificationSender.Services;

namespace LivestockTrading.Workers.NotificationSender.EventHandlers;

public class StudentCreatedNotificationHandler
{
    private readonly IPushNotificationService _pushNotificationService;
    private readonly ILogger<StudentCreatedNotificationHandler> _logger;

    public StudentCreatedNotificationHandler(
        IPushNotificationService pushNotificationService,
        ILogger<StudentCreatedNotificationHandler> logger)
    {
        _pushNotificationService = pushNotificationService;
        _logger = logger;
    }

    public Task HandleAsync(StudentCreatedEvent studentEvent)
    {
        _logger.LogInformation("Processing StudentCreatedEvent push notification for student: {StudentId} - {FirstName} {LastName}",
            studentEvent.StudentId, studentEvent.FirstName, studentEvent.LastName);

        var title = "Welcome to GlobalLivestock!";
        var body = $"Hello {studentEvent.FirstName}, your student account has been created successfully.";

        var data = new Dictionary<string, string>
        {
            { "type", "student_created" },
            { "studentId", studentEvent.StudentId.ToString() },
            { "studentNumber", studentEvent.StudentNumber ?? "" }
        };

        // In a real scenario, you would fetch the device token from your database
        // For now, we log the notification
        _logger.LogInformation("Push notification prepared - Title: {Title}, Body: {Body}, Data: {Data}",
            title, body, string.Join(", ", data.Select(kv => $"{kv.Key}={kv.Value}")));

        // Example: If you have a device token
        // return _pushNotificationService.SendPushAsync(deviceToken, title, body, data);
        return Task.CompletedTask;
    }
}
