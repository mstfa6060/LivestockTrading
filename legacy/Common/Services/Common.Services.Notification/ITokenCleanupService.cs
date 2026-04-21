namespace Common.Services.Notification;

public interface ITokenCleanupService
{
    Task CleanupInvalidTokenAsync(string token);
    Task CleanupInvalidTokensAsync(List<string> tokens);
}
