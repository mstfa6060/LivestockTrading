namespace Iam.Domain.Entities;

public class Neighborhood
{
    public int Id { get; set; }
    public int DistrictId { get; set; }
    public string Name { get; set; } = default!;
    public string? PostalCode { get; set; }
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }
    public int? GeoNameId { get; set; }
    public int SortOrder { get; set; }
}
