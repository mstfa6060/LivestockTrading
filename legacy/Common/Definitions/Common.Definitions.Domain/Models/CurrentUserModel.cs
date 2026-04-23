using Common.Definitions.Base.Enums;
using Common.Definitions.Domain.Entities;

namespace Common.Definitions.Domain.Models;

public class CurrentUserModel
{
    public Guid Id { get; set; }
    public string Username { get; set; }
    public string Email { get; set; }
    public string ImageUrl { get; set; }
    public string DisplayName { get; set; }
    public Guid TenantId { get; set; }
    public ClientPlatforms Platform { get; set; }
    public UserSources UserSource { get; set; }
    public Guid RefreshTokenId { get; set; }
    public List<string> Roles { get; set; } = new List<string>();
}