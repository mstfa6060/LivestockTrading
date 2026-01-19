using FirebaseAdmin;
using FirebaseAdmin.Messaging;
using Google.Apis.Auth.OAuth2;

namespace LivestockTrading.Workers.NotificationSender.Services;

public class FirebasePushNotificationService : IPushNotificationService
{
    private readonly ILogger<FirebasePushNotificationService> _logger;
    private readonly bool _isInitialized;

    public FirebasePushNotificationService(IConfiguration configuration, ILogger<FirebasePushNotificationService> logger)
    {
        _logger = logger;

        try
        {
            var serviceAccountPath = configuration["Firebase:ServiceAccountPath"];
            var serviceAccountJson = configuration["Firebase:ServiceAccountJson"];

            if (FirebaseApp.DefaultInstance == null)
            {
                if (!string.IsNullOrEmpty(serviceAccountJson))
                {
                    FirebaseApp.Create(new AppOptions
                    {
                        Credential = GoogleCredential.FromJson(serviceAccountJson)
                    });
                    _isInitialized = true;
                    _logger.LogInformation("Firebase initialized from JSON configuration");
                }
                else if (!string.IsNullOrEmpty(serviceAccountPath) && File.Exists(serviceAccountPath))
                {
                    FirebaseApp.Create(new AppOptions
                    {
                        Credential = GoogleCredential.FromFile(serviceAccountPath)
                    });
                    _isInitialized = true;
                    _logger.LogInformation("Firebase initialized from file: {Path}", serviceAccountPath);
                }
                else
                {
                    _logger.LogWarning("Firebase not configured. Push notifications will be logged but not sent.");
                    _isInitialized = false;
                }
            }
            else
            {
                _isInitialized = true;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to initialize Firebase");
            _isInitialized = false;
        }
    }

    public async Task<bool> SendPushAsync(string deviceToken, string title, string body, Dictionary<string, string>? data = null)
    {
        if (!_isInitialized)
        {
            _logger.LogWarning("Firebase not initialized. Push notification not sent. Title: {Title}, Body: {Body}", title, body);
            return false;
        }

        if (string.IsNullOrEmpty(deviceToken))
        {
            _logger.LogWarning("Device token is empty. Push notification not sent.");
            return false;
        }

        try
        {
            var message = new Message
            {
                Token = deviceToken,
                Notification = new Notification
                {
                    Title = title,
                    Body = body
                },
                Data = data
            };

            var response = await FirebaseMessaging.DefaultInstance.SendAsync(message);
            _logger.LogInformation("Push notification sent successfully. Response: {Response}", response);
            return true;
        }
        catch (FirebaseMessagingException ex)
        {
            _logger.LogError(ex, "Firebase messaging error. Token: {Token}, Error: {Error}", deviceToken, ex.MessagingErrorCode);
            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending push notification");
            return false;
        }
    }

    public async Task<int> SendPushToMultipleAsync(List<string> deviceTokens, string title, string body, Dictionary<string, string>? data = null)
    {
        if (!_isInitialized)
        {
            _logger.LogWarning("Firebase not initialized. Push notifications not sent.");
            return 0;
        }

        if (deviceTokens == null || deviceTokens.Count == 0)
        {
            _logger.LogWarning("No device tokens provided.");
            return 0;
        }

        var validTokens = deviceTokens.Where(t => !string.IsNullOrEmpty(t)).ToList();
        if (validTokens.Count == 0)
        {
            _logger.LogWarning("No valid device tokens provided.");
            return 0;
        }

        try
        {
            var message = new MulticastMessage
            {
                Tokens = validTokens,
                Notification = new Notification
                {
                    Title = title,
                    Body = body
                },
                Data = data
            };

            var response = await FirebaseMessaging.DefaultInstance.SendEachForMulticastAsync(message);
            _logger.LogInformation("Multicast push sent. Success: {Success}, Failure: {Failure}",
                response.SuccessCount, response.FailureCount);
            return response.SuccessCount;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending multicast push notification");
            return 0;
        }
    }
}
