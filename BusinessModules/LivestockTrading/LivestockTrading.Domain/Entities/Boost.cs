using System;

namespace LivestockTrading.Domain.Entities;

/// <summary>
/// Boost paket tanımları (Günlük, Haftalık, Mega)
/// App Store / Google Play IAP ürün tanımlarıyla eşleşir
/// </summary>
public class BoostPackage : BaseEntity
{
    /// <summary>Paket adı</summary>
    public string Name { get; set; }
    /// <summary>Paket adı çevirileri JSON</summary>
    public string NameTranslations { get; set; }
    /// <summary>Açıklama</summary>
    public string Description { get; set; }
    /// <summary>Açıklama çevirileri JSON</summary>
    public string DescriptionTranslations { get; set; }
    /// <summary>Boost süresi (saat)</summary>
    public int DurationHours { get; set; }
    /// <summary>Fiyat (USD)</summary>
    public double Price { get; set; }
    /// <summary>Para birimi</summary>
    public string Currency { get; set; }
    /// <summary>Apple App Store ürün ID</summary>
    public string AppleProductId { get; set; }
    /// <summary>Google Play ürün ID</summary>
    public string GoogleProductId { get; set; }
    /// <summary>Boost tipi</summary>
    public BoostType BoostType { get; set; }
    /// <summary>Boost skoru (sıralama ağırlığı)</summary>
    public int BoostScore { get; set; }
    /// <summary>Sıralama</summary>
    public int SortOrder { get; set; }
    /// <summary>Aktif mi?</summary>
    public bool IsActive { get; set; }

    public BoostPackage()
    {
        IsActive = true;
        Currency = "USD";
        BoostType = BoostType.Daily;
        BoostScore = 10;
    }
}

public enum BoostType
{
    Daily = 0,
    Weekly = 1,
    Mega = 2
}

/// <summary>
/// Ürün boost kaydı - hangi ürünün ne zaman boost edildiği
/// </summary>
public class ProductBoost : BaseEntity
{
    /// <summary>Ürün ID</summary>
    public Guid ProductId { get; set; }
    public Product Product { get; set; }
    /// <summary>Satıcı ID</summary>
    public Guid SellerId { get; set; }
    public Seller Seller { get; set; }
    /// <summary>Boost paketi ID</summary>
    public Guid BoostPackageId { get; set; }
    public BoostPackage BoostPackage { get; set; }
    /// <summary>IAP işlem ID</summary>
    public Guid? IAPTransactionId { get; set; }
    public IAPTransaction IAPTransaction { get; set; }
    /// <summary>Başlangıç tarihi</summary>
    public DateTime StartedAt { get; set; }
    /// <summary>Bitiş tarihi</summary>
    public DateTime ExpiresAt { get; set; }
    /// <summary>Aktif mi?</summary>
    public bool IsActive { get; set; }
    /// <summary>Boost skoru (sıralama ağırlığı)</summary>
    public int BoostScore { get; set; }

    public ProductBoost()
    {
        IsActive = true;
        StartedAt = DateTime.UtcNow;
    }
}
