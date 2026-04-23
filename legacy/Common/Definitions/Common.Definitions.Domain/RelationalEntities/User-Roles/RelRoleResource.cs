using Common.Definitions.Base.Entity;

namespace Common.Definitions.Domain.Entities;

public class RelRoleResource : CoreEntity
{
    public Guid ResourceId { get; set; }
    public Resource Resource { get; set; }
    public Guid RoleId { get; set; }
    public Role Role { get; set; }

}