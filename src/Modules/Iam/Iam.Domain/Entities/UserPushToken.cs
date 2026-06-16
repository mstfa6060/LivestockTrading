using Iam.Domain.Enums;

namespace Iam.Domain.Entities;

public class UserPushToken : BaseEntity
{
    public Guid UserId { get; set; }
    public string DeviceId { get; set; } = default!;
    public string PushToken { get; set; } = default!;
    public UserPushPlatform Platform { get; set; }
    public string AppName { get; set; } = default!;
    public DateTime? RevokedAt { get; set; }

    public User User { get; set; } = default!;
}
