namespace LivestockTrading.Domain.Entities;

/// <summary>
/// Doviz kuru bilgileri (para birimi donusumu icin)
/// </summary>
public class ExchangeRate
{
    public int Id { get; set; }
    /// <summary>Kaynak para birimi kodu (ISO 4217): USD, EUR, TRY vb.</summary>
    public string FromCurrency { get; set; }
    /// <summary>Hedef para birimi kodu (ISO 4217): USD, EUR, TRY vb.</summary>
    public string ToCurrency { get; set; }
    /// <summary>Doviz kuru</summary>
    public decimal Rate { get; set; }
    /// <summary>Son guncellenme tarihi</summary>
    public DateTime UpdatedAt { get; set; }
    /// <summary>Olusturulma tarihi</summary>
    public DateTime CreatedAt { get; set; }
}
