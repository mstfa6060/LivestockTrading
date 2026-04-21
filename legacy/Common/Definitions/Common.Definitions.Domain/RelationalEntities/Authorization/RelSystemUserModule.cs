using Common.Definitions.Base.Entity;

namespace Common.Definitions.Domain.Entities;

public class RelSystemUserModule : BaseEntity
{
    public Guid SystemAdminId { get; set; }
    public SystemAdmin SystemAdmin { get; set; }

    public Guid ModuleId { get; set; }
    public Module Module { get; set; }
}