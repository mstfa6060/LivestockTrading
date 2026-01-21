using System;

namespace LivestockTrading.Domain.Entities;

/// <summary>
/// Ürün görselleri
/// </summary>
public class ProductImage : BaseEntity
{
    public Guid ProductId { get; set; }
    public Product Product { get; set; }
    
    /// <summary>Görsel URL (FileProvider'dan)</summary>
    public string ImageUrl { get; set; }
    /// <summary>Küçük resim URL (FileProvider'dan)</summary>
    public string ThumbnailUrl { get; set; }
    /// <summary>Alt metin (SEO)</summary>
    public string AltText { get; set; }
    /// <summary>Sıralama</summary>
    public int SortOrder { get; set; }
    /// <summary>Ana görsel mi?</summary>
    public bool IsPrimary { get; set; }
    
    public ProductImage()
    {
        IsPrimary = false;
    }
}

/// <summary>
/// Ürün videoları
/// </summary>
public class ProductVideo : BaseEntity
{
    public Guid ProductId { get; set; }
    public Product Product { get; set; }
    
    /// <summary>Video URL (FileProvider'dan veya YouTube/Vimeo)</summary>
    public string VideoUrl { get; set; }
    /// <summary>Video küçük resmi URL (FileProvider'dan)</summary>
    public string ThumbnailUrl { get; set; }
    /// <summary>Video başlığı</summary>
    public string Title { get; set; }
    /// <summary>Video süresi (saniye)</summary>
    public int DurationSeconds { get; set; }
    /// <summary>Sıralama</summary>
    public int SortOrder { get; set; }
    /// <summary>Video platformu</summary>
    public VideoProvider Provider { get; set; }
}

public enum VideoProvider
{
    DirectUpload = 0,
    YouTube = 1,
    Vimeo = 2,
    DailyMotion = 3
}

/// <summary>
/// Ürün belgeleri (sertifika, kılavuz, rapor vb.)
/// </summary>
public class ProductDocument : BaseEntity
{
    public Guid ProductId { get; set; }
    public Product Product { get; set; }
    
    /// <summary>Belge URL (FileProvider'dan)</summary>
    public string DocumentUrl { get; set; }
    /// <summary>Dosya adı</summary>
    public string FileName { get; set; }
    /// <summary>Belge başlığı</summary>
    public string Title { get; set; }
    /// <summary>Belge tipi</summary>
    public DocumentType Type { get; set; }
    /// <summary>Dosya boyutu (byte)</summary>
    public long FileSizeBytes { get; set; }
    /// <summary>MIME tipi</summary>
    public string MimeType { get; set; }
    /// <summary>Sıralama</summary>
    public int SortOrder { get; set; }
}

public enum DocumentType
{
    Certificate = 0,
    Manual = 1,
    Datasheet = 2,
    WarrantyCertificate = 3,
    VeterinaryReport = 4,
    HealthCertificate = 5,
    OriginCertificate = 6,
    SafetyDataSheet = 7,
    Other = 99
}