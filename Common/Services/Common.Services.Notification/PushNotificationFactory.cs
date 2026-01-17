using Microsoft.Extensions.DependencyInjection;

namespace Common.Services.Notification;

public interface IPushNotificationFactory
{
    IPushNotificationPublisher CreatePublisher(NotificationProvider provider);
}

public class PushNotificationFactory : IPushNotificationFactory
{
    private readonly IServiceProvider _serviceProvider;

    public PushNotificationFactory(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public IPushNotificationPublisher CreatePublisher(NotificationProvider provider)
    {
        return provider switch
        {
            NotificationProvider.OneSignal => ActivatorUtilities.CreateInstance<OneSignalPushNotificationPublisher>(_serviceProvider),
            NotificationProvider.Firebase => ActivatorUtilities.CreateInstance<FirebasePushNotificationPublisher>(_serviceProvider),
            _ => throw new ArgumentException($"Unsupported notification provider: {provider}")
        };
    }
}
