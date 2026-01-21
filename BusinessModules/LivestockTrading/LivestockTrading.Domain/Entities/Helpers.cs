using System;
using System.Collections.Generic;

namespace LivestockTrading.Domain.Entities;

/// <summary>
/// Para birimi bilgileri (çoklu para birimi desteği için)
/// </summary>
public class Currency : BaseEntity
{
    /// <summary>Para birimi kodu (ISO 4217): USD, EUR, TRY vb.</summary>
    public string Code { get; set; }
    /// <summary>Sembol: $, €, ₺ vb.</summary>
    public string Symbol { get; set; }
    /// <summary>Para birimi adı</summary>
    public string Name { get; set; }
    /// <summary>USD'ye göre dönüşüm kuru</summary>
    public decimal ExchangeRateToUSD { get; set; }
    /// <summary>Son güncellenme tarihi</summary>
    public DateTime LastUpdated { get; set; }
    /// <summary>Aktif mi?</summary>
    public bool IsActive { get; set; }
    
    public Currency()
    {
        IsActive = true;
        LastUpdated = DateTime.UtcNow;
    }
}

/// <summary>
/// Ülke bilgileri (konum bazlı özellikler için)
/// </summary>
public class Country : BaseEntity
{
    /// <summary>Ülke kodu (ISO 3166-1 alpha-2)</summary>
    public string Code { get; set; }
    /// <summary>Ülke adı</summary>
    public string Name { get; set; }
    /// <summary>Yerel dilde adı</summary>
    public string NativeName { get; set; }
    /// <summary>Telefon kodu (+1, +90 vb.)</summary>
    public string PhoneCode { get; set; }
    /// <summary>Varsayılan para birimi</summary>
    public string DefaultCurrency { get; set; }
    /// <summary>Aktif mi?</summary>
    public bool IsActive { get; set; }
    
    public Country()
    {
        IsActive = true;
    }
}

/// <summary>
/// Ödeme yöntemi bilgileri
/// </summary>
public class PaymentMethod : BaseEntity
{
    /// <summary>Ödeme yöntemi adı</summary>
    public string Name { get; set; }
    /// <summary>Kod (CREDIT_CARD, BANK_TRANSFER, PAYPAL vb.)</summary>
    public string Code { get; set; }
    /// <summary>Açıklama</summary>
    public string Description { get; set; }
    /// <summary>İkon URL (FileProvider'dan)</summary>
    public string IconUrl { get; set; }
    /// <summary>Manuel doğrulama gerekiyor mu?</summary>
    public bool RequiresManualVerification { get; set; }
    /// <summary>Aktif mi?</summary>
    public bool IsActive { get; set; }
    /// <summary>Desteklenen ülkeler JSON (ülke kodları)</summary>
    public string SupportedCountries { get; set; }
    /// <summary>Desteklenen para birimleri JSON</summary>
    public string SupportedCurrencies { get; set; }
    /// <summary>İşlem ücreti oranı (%)</summary>
    public decimal? TransactionFeePercentage { get; set; }
    /// <summary>Sabit işlem ücreti</summary>
    public decimal? FixedTransactionFee { get; set; }
    
    public PaymentMethod()
    {
        IsActive = true;
        RequiresManualVerification = false;
    }
}

/// <summary>
/// Kargo firması bilgileri
/// </summary>
public class ShippingCarrier : BaseEntity
{
    /// <summary>Kargo firması adı</summary>
    public string Name { get; set; }
    /// <summary>Kargo firması kodu</summary>
    public string Code { get; set; }
    /// <summary>Web sitesi</summary>
    public string Website { get; set; }
    /// <summary>Takip URL şablonu ({trackingNumber} yerine takip no gelecek)</summary>
    public string TrackingUrlTemplate { get; set; }
    /// <summary>Aktif mi?</summary>
    public bool IsActive { get; set; }
    /// <summary>Desteklenen ülkeler JSON</summary>
    public string SupportedCountries { get; set; }
    
    public ShippingCarrier()
    {
        IsActive = true;
    }
}

/// <summary>
/// Sıkça Sorulan Sorular (ürün/kategori bazlı)
/// </summary>
public class FAQ : BaseEntity
{
    /// <summary>Soru</summary>
    public string Question { get; set; }
    /// <summary>Cevap</summary>
    public string Answer { get; set; }
    /// <summary>Kategori ID (eğer kategoriye özelse)</summary>
    public Guid? CategoryId { get; set; }
    public Category Category { get; set; }
    /// <summary>Ürün ID (eğer ürüne özelse)</summary>
    public Guid? ProductId { get; set; }
    public Product Product { get; set; }
    /// <summary>Sıralama</summary>
    public int SortOrder { get; set; }
    /// <summary>Aktif mi?</summary>
    public bool IsActive { get; set; }
    /// <summary>Soru çevirileri JSON</summary>
    public string QuestionTranslations { get; set; }
    /// <summary>Cevap çevirileri JSON</summary>
    public string AnswerTranslations { get; set; }
    
    public FAQ()
    {
        IsActive = true;
    }
}

/// <summary>
/// Promosyon banner'ları
/// </summary>
public class Banner : BaseEntity
{
    /// <summary>Başlık</summary>
    public string Title { get; set; }
    /// <summary>Açıklama</summary>
    public string Description { get; set; }
    /// <summary>Görsel URL (FileProvider'dan)</summary>
    public string ImageUrl { get; set; }
    /// <summary>Hedef URL (tıklanınca nereye gidecek)</summary>
    public string TargetUrl { get; set; }
    /// <summary>Banner pozisyonu</summary>
    public BannerPosition Position { get; set; }
    /// <summary>Başlangıç tarihi</summary>
    public DateTime StartDate { get; set; }
    /// <summary>Bitiş tarihi</summary>
    public DateTime EndDate { get; set; }
    /// <summary>Aktif mi?</summary>
    public bool IsActive { get; set; }
    /// <summary>Gösterim sırası</summary>
    public int DisplayOrder { get; set; }
    /// <summary>Tıklanma sayısı</summary>
    public int ClickCount { get; set; }
    /// <summary>Gösterim sayısı</summary>
    public int ImpressionCount { get; set; }
    
    public Banner()
    {
        IsActive = true;
        StartDate = DateTime.UtcNow;
    }
}

public enum BannerPosition
{
    Homepage = 0,
    CategoryPage = 1,
    ProductPage = 2,
    SearchResults = 3,
    Sidebar = 4
}
