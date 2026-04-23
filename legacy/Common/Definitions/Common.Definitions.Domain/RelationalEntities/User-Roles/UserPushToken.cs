using Common.Definitions.Base.Entity;

namespace Common.Definitions.Domain.Entities;

public class UserPushToken : BaseEntity
{
    public Guid UserId { get; set; }
    public User User { get; set; }
    public string DeviceId { get; set; }
    public string PushToken { get; set; }
    public UserPushPlatform Platform { get; set; }
    public string AppName { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? RevokedAt { get; set; }
}
public enum UserPushPlatform
{
    Expo = 0,
    Firebase = 1,
    OneSignal = 2,
    Apns = 3,
    WebPush = 4
}