namespace Iam.Domain.Entities;

public class UserRole : BaseEntity
{
    public Guid UserId { get; set; }
    public Guid RoleId { get; set; }
    public Guid ModuleId { get; set; }

    public User User { get; set; } = default!;
    public Role Role { get; set; } = default!;
    public IamModule Module { get; set; } = default!;
}
