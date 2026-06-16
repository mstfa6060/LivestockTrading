namespace Iam.Domain.Entities;

public class IamModule
{
    public Guid Id { get; set; }
    public string Name { get; set; } = default!;

    public ICollection<UserRole> UserRoles { get; set; } = [];
}
