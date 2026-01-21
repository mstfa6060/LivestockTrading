using System;

namespace LivestockTrading.Domain.Entities;

/// <summary>
/// Platform tarafından desteklenen diller
/// Frontend i18n için hangi dillerin aktif olduğunu belirler
/// Asıl çeviriler frontend'de (i18n files) yapılır
/// </summary>
public class Language : BaseEntity
{
    /// <summary>Dil kodu (ISO 639-1: en, tr, es, fr, de, ar, zh, hi, pt, ja, ko, ru, it vb.)</summary>
    public string Code { get; set; }
    
    /// <summary>Dil adı (İngilizce)</summary>
    public string Name { get; set; }
    
    /// <summary>Yerel dilde adı</summary>
    public string NativeName { get; set; }
    
    /// <summary>Sağdan sola mı? (Arapça, İbranice, Farsça gibi)</summary>
    /// <remarks>Frontend i18n için RTL layout ayarı</remarks>
    public bool IsRightToLeft { get; set; }
    
    /// <summary>Aktif mi?</summary>
    public bool IsActive { get; set; }
    
    /// <summary>Varsayılan dil mi?</summary>
    public bool IsDefault { get; set; }
    
    /// <summary>Sıralama</summary>
    public int SortOrder { get; set; }
    
    /// <summary>Bayrak ikonu URL (FileProvider'dan)</summary>
    public string FlagIconUrl { get; set; }
    
    public Language()
    {
        IsActive = true;
        IsDefault = false;
        IsRightToLeft = false;
    }
}
