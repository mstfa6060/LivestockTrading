using System.ComponentModel.DataAnnotations.Schema;
using Common.Definitions.Base.Entity;
using NetTopologySuite.Geometries;



namespace Common.Definitions.Domain.Entities;
public class UserLocation : BaseEntity, ITenantEntity
{
    public Guid UserId { get; set; }
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public DateTime Timestamp { get; set; }  // Konumun kaydedildiği an


    [ForeignKey("UserId")]
    public User User { get; set; }


    public Guid CompanyId { get; set; }
    public Company Company { get; set; }

    public Guid GetTenantId() => this.CompanyId;
    public string GetTenantPropertyName() => "";
    public object GetTenantEntity()
    {
        return null;
    }

}
