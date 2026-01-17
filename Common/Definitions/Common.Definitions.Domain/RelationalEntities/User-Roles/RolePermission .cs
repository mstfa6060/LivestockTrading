using System.ComponentModel.DataAnnotations.Schema;
using Common.Definitions.Base.Entity;

namespace Common.Definitions.Domain.Entities;

public class RolePermission : BaseEntity
{
    public Guid RoleId { get; set; }
    public string Permission { get; set; } // "ManageUsers", "PostJobs" vb.

    [ForeignKey("RoleId")]
    public Role Role { get; set; }
}
