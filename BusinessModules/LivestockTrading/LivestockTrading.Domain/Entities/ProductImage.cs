using System;

namespace LivestockTrading.Domain.Entities;

/// <summary>
/// Urun gorseli
/// Urunlere ait gorsellerin metadata bilgileri
/// </summary>
public class ProductImage : BaseEntity
{
    /// <summary>Urun ID</summary>
    public Guid ProductId { get; set; }
    public Product Product { get; set; }

    /// <summary>Gorsel URL (FileProvider'dan)</summary>
    public string ImageUrl { get; set; }

    /// <summary>Kucuk gorsel URL</summary>
    public string ThumbnailUrl { get; set; }

    /// <summary>Alternatif metin (erisilebilirlik)</summary>
    public string AltText { get; set; }

    /// <summary>Siralama</summary>
    public int SortOrder { get; set; }

    /// <summary>Ana gorsel mi?</summary>
    public bool IsPrimary { get; set; }

    public ProductImage()
    {
        SortOrder = 0;
        IsPrimary = false;
    }
}
