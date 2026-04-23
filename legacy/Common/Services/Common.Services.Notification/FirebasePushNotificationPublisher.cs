using FirebaseAdmin;
using FirebaseAdmin.Messaging;
using Google.Apis.Auth.OAuth2;
using Microsoft.Extensions.Configuration;

namespace Common.Services.Notification;

public class FirebasePushNotificationPublisher : IPushNotificationPublisher
{
    private readonly FirebaseMessaging _messaging;

    public FirebasePushNotificationPublisher(IConfiguration config)
    {
        if (FirebaseApp.DefaultInstance == null)
        {
            var path = config["Firebase:ServiceAccountPath"];
            if (!string.IsNullOrEmpty(path))
                FirebaseApp.Create(new AppOptions { Credential = GoogleCredential.FromFile(path) });
        }

        _messaging = FirebaseMessaging.DefaultInstance;
    }

    public Task SendPushAsync(string userId, string title, string body, string jobId)
    {
        var message = new Message
        {
            Topic = "all_users",
            Notification = new FirebaseAdmin.Messaging.Notification
            {
                Title = title,
                Body = body
            },
            Data = new Dictionary<string, string>
            {
                { "jobId", jobId },
                { "title", title },
                { "body", body }
            },
            Android = new AndroidConfig
            {
                Priority = Priority.High,
                Notification = new AndroidNotification
                {
                    Title = title,
                    Body = body,
                    Icon = "ic_notification",
                    Sound = "default",
                    ChannelId = "general_notifications"
                }
            },
            Apns = new ApnsConfig
            {
                Aps = new Aps
                {
                    Alert = new ApsAlert
                    {
                        Title = title,
                        Body = body
                    },
                    Sound = "default",
                    Badge = 1
                }
            }
        };
        return _messaging.SendAsync(message);
    }

    public async Task SendPushToTokenAsync(string token, string title, string body, string jobId)
    {
        if (string.IsNullOrEmpty(token))
        {
            Console.WriteLine($"[Firebase] ⚠️ Boş token - gönderim atlandı");
            return;
        }

        var message = new Message
        {
            Token = token,
            Notification = new FirebaseAdmin.Messaging.Notification
            {
                Title = title,
                Body = body
            },
            Data = new Dictionary<string, string>
            {
                { "jobId", jobId },
                { "title", title },
                { "body", body },
                { "type", "job-notification" }
            },
            Android = new AndroidConfig
            {
                Priority = Priority.High,
                Notification = new AndroidNotification
                {
                    Title = title,
                    Body = body,
                    Icon = "ic_notification",
                    Sound = "default",
                    ChannelId = "job_notifications"
                }
            },
            Apns = new ApnsConfig
            {
                Aps = new Aps
                {
                    Alert = new ApsAlert
                    {
                        Title = title,
                        Body = body
                    },
                    Sound = "default",
                    Badge = 1
                }
            }
        };

        try
        {
            var result = await _messaging.SendAsync(message);
            Console.WriteLine($"[Firebase]  Push gönderildi - Token: {token.Substring(0, Math.Min(20, token.Length))}..., MessageId: {result}");
        }
        catch (FirebaseMessagingException ex)
        {
            // Geçersiz token hatalarını handle et
            if (ex.MessagingErrorCode == MessagingErrorCode.Unregistered ||
                ex.MessagingErrorCode == MessagingErrorCode.InvalidArgument)
            {
                Console.WriteLine($"[Firebase] ⚠️ Geçersiz token - Token: {token.Substring(0, Math.Min(20, token.Length))}..., Hata: {ex.MessagingErrorCode} - {ex.Message}");
                // Burada token'ı veritabanından silmek için bir callback veya event tetiklenebilir
                throw new InvalidTokenException($"Geçersiz FCM token: {ex.MessagingErrorCode}", ex);
            }
            else
            {
                Console.WriteLine($"[Firebase] ❌ Push hatası - Token: {token.Substring(0, Math.Min(20, token.Length))}..., Hata: {ex.MessagingErrorCode} - {ex.Message}");
                throw;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[Firebase] ❌ Beklenmeyen hata - Token: {token.Substring(0, Math.Min(20, token.Length))}..., Hata: {ex.Message}");
            throw;
        }
    }

    public async Task SendPushToMultipleTokensAsync(List<string> tokens, string title, string body, string jobId, Dictionary<string, string>? additionalData = null)
    {
        if (tokens == null || !tokens.Any())
        {
            Console.WriteLine($"[Firebase] ⚠️ Boş token listesi - gönderim atlandı");
            return;
        }

        // Geçerli token'ları filtrele
        var validTokens = tokens.Where(t => !string.IsNullOrEmpty(t)).ToList();

        if (!validTokens.Any())
        {
            Console.WriteLine($"[Firebase] ⚠️ Geçerli token bulunamadı - gönderim atlandı");
            return;
        }

        // Base data dictionary oluştur
        var messageData = new Dictionary<string, string>
        {
            { "jobId", jobId },
            { "title", title },
            { "body", body },
            { "type", "job-notification" }
        };

        // additionalData varsa birleştir
        if (additionalData != null)
        {
            foreach (var kvp in additionalData)
            {
                messageData[kvp.Key] = kvp.Value;
            }
            Console.WriteLine($"[Firebase] 📎 AdditionalData eklendi: {string.Join(", ", additionalData.Keys)}");
        }

        var message = new MulticastMessage
        {
            Tokens = validTokens,
            Notification = new FirebaseAdmin.Messaging.Notification
            {
                Title = title,
                Body = body
            },
            Data = messageData,
            Android = new AndroidConfig
            {
                Priority = Priority.High,
                Notification = new AndroidNotification
                {
                    Title = title,
                    Body = body,
                    Icon = "ic_notification",
                    Sound = "default",
                    ChannelId = "job_notifications"
                }
            },
            Apns = new ApnsConfig
            {
                Aps = new Aps
                {
                    Alert = new ApsAlert
                    {
                        Title = title,
                        Body = body
                    },
                    Sound = "default",
                    Badge = 1
                }
            }
        };

        try
        {
            var result = await _messaging.SendEachForMulticastAsync(message);
            Console.WriteLine($"[Firebase]  Multicast gönderildi - Başarılı: {result.SuccessCount}, Başarısız: {result.FailureCount}");

            // Başarısız token'ları logla
            if (result.FailureCount > 0)
            {
                for (int i = 0; i < result.Responses.Count; i++)
                {
                    if (!result.Responses[i].IsSuccess)
                    {
                        var failedToken = validTokens[i];
                        Console.WriteLine($"[Firebase] ❌ Başarısız token: {failedToken.Substring(0, Math.Min(20, failedToken.Length))}..., Hata: {result.Responses[i].Exception?.Message}");
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[Firebase] ❌ Multicast hatası: {ex.Message}");
            throw;
        }
    }
}

// Geçersiz token exception'ı
public class InvalidTokenException : Exception
{
    public InvalidTokenException(string message, Exception innerException) : base(message, innerException)
    {
    }
}
