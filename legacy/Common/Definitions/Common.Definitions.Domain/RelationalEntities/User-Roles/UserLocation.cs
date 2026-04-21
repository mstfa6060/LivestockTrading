using System.ComponentModel.DataAnnotations.Schema;
using Common.Definitions.Base.Entity;
using NetTopologySuite.Geometries;



namespace Common.Definitions.Domain.Entities;
public class UserLocation : BaseEntity
{
    public Guid UserId { get; set; }
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public DateTime Timestamp { get; set; }  // Konumun kaydedildiği an


    [ForeignKey("UserId")]
    public User User { get; set; }
}
