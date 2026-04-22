namespace Livestock.Domain.Entities;

public class ShippingZone : BaseEntity
{
    public Guid? SellerId { get; set; }
    public string Name { get; set; } = default!;
    public string CountryCodes { get; set; } = default!;
    public bool IsActive { get; set; } = true;

    public Seller? Seller { get; set; }
    public ICollection<ShippingRate> Rates { get; set; } = [];
}
