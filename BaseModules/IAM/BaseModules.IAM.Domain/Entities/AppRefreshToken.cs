using Common.Definitions.Base.Entity;

namespace BaseModules.IAM.Domain.Entities;

public class AppRefreshToken : BaseEntity
{
    public Guid UserId { get; set; }
    public string Token { get; set; }
    public DateTime ExpiresAt { get; set; }
    public string UserPlatform { get; set; } // iOS, Web, Android
    public string? IpAddress { get; set; }
}
