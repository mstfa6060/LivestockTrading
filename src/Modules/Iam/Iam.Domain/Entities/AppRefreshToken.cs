using Iam.Domain.Enums;

namespace Iam.Domain.Entities;

public class AppRefreshToken : BaseEntity
{
    public Guid UserId { get; set; }
    public string Token { get; set; } = default!;
    public DateTime ExpiresAt { get; set; }
    public ClientPlatforms Platform { get; set; }
    public string? IpAddress { get; set; }

    public User User { get; set; } = default!;
}
