namespace Livestock.Domain.Entities;

public class ShippingZone : BaseEntity
{
    public Guid? SellerId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? CountryCodes { get; set; }
    public bool IsActive { get; set; } = true;

    public ICollection<ShippingRate> ShippingRates { get; set; } = [];
}
