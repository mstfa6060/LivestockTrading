---
paths:
  - "**/RequestHandlers/**"
---
# Cok Ulkeli Filtreleme (Multi-Country Filtering)

Platform 50+ dil ve cok sayida ulkeyi destekler. Kullanicilar default ulkelerinin ilanlarini gorur veya frontend'den baska bir ulke secebilir.

**Temel Kurallar:**
1. Urunler `Location.CountryCode` (ISO 3166-1 alpha-2) uzerinden ulkeye baglidir
2. Frontend `countryCode` parametresi gonderirse sadece o ulkenin urunleri doner
3. `countryCode` bos ise tum urunler doner (global gorunum)

**Listeleme Endpoint'lerinde Kullanim:**
```csharp
// Models.cs
public class RequestModel : IRequestModel
{
    /// <summary>Ulke kodu filtresi (ISO 3166-1 alpha-2, orn: "TR", "US", "DE")</summary>
    public string CountryCode { get; set; }
    public XSorting Sorting { get; set; }
    public List<XFilterItem> Filters { get; set; }
    public XPageRequest PageRequest { get; set; }
}

// DataAccess.cs - Include ile Location yuklenir
var query = _dbContext.Products
    .AsNoTracking()
    .Include(p => p.Location)
    .Where(p => !p.IsDeleted);

// Ulke filtresi uygulanir
if (!string.IsNullOrWhiteSpace(countryCode))
{
    query = query.Where(p => p.Location != null && p.Location.CountryCode == countryCode);
}
```

**ResponseModel'de Ulke Bilgisi:**
```csharp
public class ResponseModel : IResponseModel<Array>
{
    // ... diger alanlar ...
    public Guid LocationId { get; set; }
    public string LocationCountryCode { get; set; }
    public string LocationCity { get; set; }
}

// Mapper'da
LocationCountryCode = entity.Location?.CountryCode,
LocationCity = entity.Location?.City,
```

**Kullanici Ulke Tercihi:**
- `User.CountryId`: Kullanicinin varsayilan ulkesi
- `User.LastViewingCountryId`: Son goruntuledigi ulke (frontend guncelleyebilir)
- Frontend ulke degistirdiginde `countryCode` parametresini request body'de gonderir

# Coklu Dil Destegi (Multi-Language Translations)

Kategoriler ve diger global entity'ler coklu dil destegine sahiptir. Ceviriler JSON formatinda saklanir.

**Entity'de Ceviri Alanlari:**
```csharp
// Category entity ornegi
public string Name { get; set; }  // Varsayilan isim
public string NameTranslations { get; set; }  // JSON: {"en":"Livestock","tr":"Hayvancilik","de":"Viehzucht"}
public string DescriptionTranslations { get; set; }  // JSON cevirileri
```

**Backend Ceviri Cozumleme:**
```csharp
// Models.cs - LanguageCode parametresi ekle
public class RequestModel : IRequestModel
{
    /// <summary>Dil kodu (ISO 639-1, orn: "tr", "en", "de")</summary>
    public string LanguageCode { get; set; }
    // ... diger alanlar
}

// Mapper.cs - TranslationHelper kullan
using LivestockTrading.Application.Extensions;

public ResponseModel MapToResponse(Category entity, string languageCode = null)
{
    return new ResponseModel
    {
        Name = GetTranslatedName(entity, languageCode),
        // ...
    };
}

private static string GetTranslatedName(Category c, string languageCode)
{
    if (string.IsNullOrWhiteSpace(languageCode))
        return c.Name;
    return TranslationHelper.GetTranslation(c.NameTranslations, languageCode, c.Name);
}

// Handler.cs - languageCode'u Mapper'a gec
var response = mapper.MapToResponse(entity, req.LanguageCode);
```

**TranslationHelper Davranisi:**
1. `languageCode` ile tam eslesme arar (orn: "tr")
2. Bulamazsa buyuk/kucuk harf duyarsiz arar
3. Bulamazsa Ingilizce ("en") fallback
4. O da yoksa ilk mevcut ceviriyi dondurur
5. Hicbiri yoksa `fallbackValue` (genellikle entity.Name) doner

**Ceviri Destekleyen Entity'ler:**
- `Category`: NameTranslations, DescriptionTranslations
- Diger entity'lere ayni pattern ile eklenebilir
