using System;

namespace LivestockTrading.Domain.Entities;

/// <summary>
/// Ülke/Bölge bazlı vergi oranları
/// </summary>
public class TaxRate : BaseEntity
{
    public string CountryCode { get; set; }
    public string StateCode { get; set; }
    public string TaxName { get; set; }
    public decimal Rate { get; set; }
    public TaxType Type { get; set; }
    public Guid? CategoryId { get; set; }
    public bool IsActive { get; set; }
    public DateTime? ValidFrom { get; set; }
    public DateTime? ValidUntil { get; set; }

    public TaxRate()
    {
        IsActive = true;
    }
}

public enum TaxType
{
    VAT = 0,
    SalesTax = 1,
    GST = 2,
    ImportDuty = 3,
    CustomsDuty = 4,
    Other = 99
}
