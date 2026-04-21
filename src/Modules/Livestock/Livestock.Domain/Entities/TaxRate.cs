using Livestock.Domain.Enums;

namespace Livestock.Domain.Entities;

public class TaxRate : BaseEntity
{
    public string CountryCode { get; set; } = string.Empty;
    public string? StateCode { get; set; }
    public string TaxName { get; set; } = string.Empty;
    public double Rate { get; set; }
    public TaxType Type { get; set; } = TaxType.VAT;
    public Guid? CategoryId { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime? ValidFrom { get; set; }
    public DateTime? ValidUntil { get; set; }
}
