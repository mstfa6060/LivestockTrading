using Livestock.Domain.Enums;

namespace Livestock.Domain.Entities;

public class MachineryInfo : BaseEntity
{
    public Guid ProductId { get; set; }
    public MachineryType MachineryType { get; set; }
    public string? Manufacturer { get; set; }
    public string? Model { get; set; }
    public int? Year { get; set; }
    public decimal? HorsePower { get; set; }
    public int? WorkingHours { get; set; }
    public string? FuelType { get; set; }
    public string? Dimensions { get; set; }
    public decimal? WeightKg { get; set; }
    public bool HasWarranty { get; set; }
    public DateTime? WarrantyExpiresAt { get; set; }
    public string? SerialNumber { get; set; }
    public string? AdditionalFeatures { get; set; }

    public Product Product { get; set; } = null!;
}
