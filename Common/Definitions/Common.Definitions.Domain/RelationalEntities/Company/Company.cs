using System.ComponentModel.DataAnnotations.Schema;
using Common.Definitions.Base.Entity;



namespace Common.Definitions.Domain.Entities;

[Table("AppCompanies")]
public class Company : BaseEntity, ITenantEntity
{
    public string Name { get; set; }
    public string TaxNumber { get; set; } // Vergi Numarası
    public string Address { get; set; }
    public string Phone { get; set; }

    public ICollection<User> Users { get; set; } // 👈 Navigation property varsa daha iyi

    public Guid GetTenantId() => this.Id;
    public string GetTenantPropertyName() => "";
    public object GetTenantEntity()
    {
        return null;
    }
    // public List<ResourcePermission> ResourcePermissions { get; set; }

}
