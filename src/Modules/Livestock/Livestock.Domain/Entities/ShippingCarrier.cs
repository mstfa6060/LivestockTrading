namespace Livestock.Domain.Entities;

public class ShippingCarrier : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public string? Website { get; set; }
    public string? TrackingUrlTemplate { get; set; }
    public bool IsActive { get; set; } = true;
    public string? SupportedCountries { get; set; }

    public ICollection<ShippingRate> ShippingRates { get; set; } = [];
}
