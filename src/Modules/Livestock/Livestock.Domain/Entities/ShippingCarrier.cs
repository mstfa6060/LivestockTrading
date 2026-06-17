namespace Livestock.Domain.Entities;

public class ShippingCarrier : BaseEntity
{
    public string Name { get; set; } = default!;
    public string Code { get; set; } = default!;
    public string? Website { get; set; }
    public string? TrackingUrlTemplate { get; set; }
    public bool IsActive { get; set; } = true;
    public string? SupportedCountries { get; set; }

    public ICollection<ShippingRate> Rates { get; set; } = [];
}
