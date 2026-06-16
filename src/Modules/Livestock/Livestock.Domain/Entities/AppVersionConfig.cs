namespace Livestock.Domain.Entities;

public class AppVersionConfig : BaseEntity
{
    // 0=Web, 1=Android, 2=iOS
    public int Platform { get; set; }
    public string MinSupportedVersion { get; set; } = string.Empty;
    public string LatestVersion { get; set; } = string.Empty;
    public string StoreUrl { get; set; } = string.Empty;
    public string? UpdateMessage { get; set; }
    public bool IsActive { get; set; } = true;
}
