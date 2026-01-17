using System.ComponentModel.DataAnnotations.Schema;
using Common.Definitions.Base.Entity;



namespace Common.Definitions.Domain.Entities;

public class UserRole : BaseEntity, ITenantEntity
{
    public Guid UserId { get; set; }
    public Guid RoleId { get; set; }

    [ForeignKey("UserId")]
    public User User { get; set; }

    [ForeignKey("RoleId")]
    public Role Role { get; set; }

    public Guid CompanyId { get; set; }
    public Company Company { get; set; }

    // Property ekle (CompanyId'nin altına)
    public Guid ModuleId { get; set; }

    // Navigation property ekle (Company'nin altına)
    public Module Module { get; set; }

    // ITenantEntity implementasyonu
    public Guid GetTenantId() => CompanyId;
    public string GetTenantPropertyName() => nameof(Company);
    public object GetTenantEntity() => Company;
}




public class Role : BaseEntity, ITenantEntity
{
    public string Name { get; set; } // "Admin", "Employer", "Worker" vb.

    public bool IsSystemRole { get; set; } = false; // True: Sistem tarafından tanımlı rol

    public Guid CompanyId { get; set; }
    public Company Company { get; set; }


    public Guid GetTenantId() => this.CompanyId;
    public string GetTenantPropertyName() => "CompanyId";
    public object GetTenantEntity() => null;

    public List<RolePermission> RolePermissions { get; set; } = new List<RolePermission>();
}
