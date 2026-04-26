namespace Iam.Domain.Entities;

public class District
{
    public int Id { get; set; }
    public int ProvinceId { get; set; }
    public string Name { get; set; } = default!;
    public string? NameTranslations { get; set; }
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }
    public long? Population { get; set; }
    public string? Timezone { get; set; }
    public int? GeoNameId { get; set; }
    public int SortOrder { get; set; }
}
