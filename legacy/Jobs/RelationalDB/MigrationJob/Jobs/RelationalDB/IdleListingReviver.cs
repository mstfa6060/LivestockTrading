using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using LivestockTrading.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace MigrationJob.SeedData;

public class IdleListingReviver
{
    private readonly DbContext _db;

    public IdleListingReviver(DbContext db)
    {
        _db = db;
    }

    /// <summary>
    /// Atıl (Draft/PendingApproval/Inactive/Expired veya Active+süresi dolmuş) ilanları
    /// güzel başlık/açıklama ile yeniden yayına alır.
    /// </summary>
    /// <param name="commit">true ise DB'ye yazar; false ise (dry-run) sadece rapor basar.</param>
    public async Task RunAsync(bool commit)
    {
        var now = DateTime.UtcNow;
        var idleStatuses = new[]
        {
            ProductStatus.Draft,
            ProductStatus.PendingApproval,
            ProductStatus.Inactive,
            ProductStatus.Expired
        };

        // 1. Atıl ilanları çek (include navigation props)
        var products = await _db.Set<Product>()
            .Include(p => p.Category)
            .Include(p => p.Brand)
            .Include(p => p.Seller)
            .Include(p => p.Location)
            .Where(p => !p.IsDeleted &&
                (idleStatuses.Contains(p.Status) ||
                 (p.Status == ProductStatus.Active && p.ExpiresAt != null && p.ExpiresAt < now)))
            .ToListAsync();

        if (products.Count == 0)
        {
            Console.WriteLine("  Atıl ilan bulunamadı. İşlem yapılacak bir şey yok.");
            return;
        }

        Console.WriteLine($"  Toplam {products.Count} atıl ilan bulundu.");

        // 2. AnimalInfo'ları toplu çek
        var productIds = products.Select(p => p.Id).ToList();
        var animalInfos = await _db.Set<AnimalInfo>()
            .Where(a => productIds.Contains(a.ProductId))
            .ToListAsync();
        var animalInfoMap = animalInfos.ToDictionary(a => a.ProductId);

        // 3. Mevcut slug'ları çek (collision kontrolü için)
        var existingSlugs = new HashSet<string>(
            await _db.Set<Product>()
                .Where(p => !p.IsDeleted)
                .Select(p => p.Slug)
                .ToListAsync(),
            StringComparer.OrdinalIgnoreCase);

        // 4. Her ilan için yeni içerik üret
        var sampleCount = 0;
        var statusBreakdown = new Dictionary<string, int>();

        foreach (var product in products)
        {
            var oldStatus = product.Status.ToString();
            if (product.Status == ProductStatus.Active)
                oldStatus = "Active(Expired)";

            if (!statusBreakdown.ContainsKey(oldStatus))
                statusBreakdown[oldStatus] = 0;
            statusBreakdown[oldStatus]++;

            animalInfoMap.TryGetValue(product.Id, out var animalInfo);

            var oldTitle = product.Title;
            var oldShortDesc = product.ShortDescription;

            // Yeni içerik üret
            product.Title = GenerateTitle(product, animalInfo);
            product.ShortDescription = GenerateShortDescription(product, animalInfo);
            product.Description = GenerateDescription(product, animalInfo);

            // Slug regenerate
            var newSlug = Slugify(product.Title);
            if (existingSlugs.Contains(newSlug) && !string.Equals(product.Slug, newSlug, StringComparison.OrdinalIgnoreCase))
            {
                newSlug = $"{newSlug}-{Guid.NewGuid().ToString("N")[..6]}";
            }
            existingSlugs.Remove(product.Slug ?? "");
            existingSlugs.Add(newSlug);
            product.Slug = newSlug;

            // SEO meta
            product.MetaTitle = product.Title.Length > 60 ? product.Title[..60] : product.Title;
            product.MetaDescription = product.ShortDescription.Length > 160
                ? product.ShortDescription[..160]
                : product.ShortDescription;

            // Durum güncelle
            product.Status = ProductStatus.Active;
            product.PublishedAt ??= now;
            product.ExpiresAt = now.AddDays(60);
            product.IsInStock = true;
            product.UpdatedAt = now;

            // Dry-run: ilk 10 örneği göster
            if (sampleCount < 10)
            {
                Console.WriteLine();
                Console.WriteLine($"  ── Örnek #{sampleCount + 1} (ID: {product.Id}) ──");
                Console.WriteLine($"  Eski Durum  : {oldStatus}");
                Console.WriteLine($"  Eski Başlık : {Truncate(oldTitle, 80)}");
                Console.WriteLine($"  Yeni Başlık : {product.Title}");
                Console.WriteLine($"  Slug        : {product.Slug}");
                Console.WriteLine($"  Kısa Açık.  : {Truncate(product.ShortDescription, 120)}");
                Console.WriteLine($"  Fiyat       : {product.BasePrice:N0} {product.Currency}");
                Console.WriteLine($"  Kategori    : {product.Category?.Name ?? "—"}");
                Console.WriteLine($"  Satıcı      : {product.Seller?.BusinessName ?? "—"}");
                Console.WriteLine($"  Konum       : {product.Location?.City ?? "—"}, {product.Location?.CountryCode ?? "—"}");
                if (animalInfo != null)
                    Console.WriteLine($"  Hayvan      : {animalInfo.BreedName ?? "—"}, {GenderLabel(animalInfo.Gender)}, {animalInfo.AgeMonths ?? 0} ay, {animalInfo.WeightKg ?? 0} kg");
                sampleCount++;
            }
        }

        // Özet
        Console.WriteLine();
        Console.WriteLine("  ═══ ÖZET ═══");
        Console.WriteLine($"  Toplam etkilenen ilan : {products.Count}");
        foreach (var kv in statusBreakdown.OrderByDescending(x => x.Value))
            Console.WriteLine($"    {kv.Key,-20}: {kv.Value}");
        Console.WriteLine($"  Hayvan bilgisi olan   : {animalInfos.Count}");
        Console.WriteLine($"  Yeni ExpiresAt        : {now.AddDays(60):yyyy-MM-dd}");

        if (!commit)
        {
            Console.WriteLine();
            Console.WriteLine("  *** DRY-RUN: Hiçbir değişiklik kaydedilmedi. ***");
            Console.WriteLine("  Gerçek yazma için: --revive-idle-listings-commit");
            return;
        }

        // 5. Commit
        Console.WriteLine();
        Console.WriteLine("  Değişiklikler kaydediliyor...");
        await _db.SaveChangesAsync();
        Console.WriteLine($"  OK: {products.Count} ilan başarıyla güncellendi ve yayına alındı.");
    }

    // ───────────── Template Functions ─────────────

    private static string GenerateTitle(Product p, AnimalInfo? ai)
    {
        var parts = new List<string>();

        if (ai != null && !string.IsNullOrWhiteSpace(ai.BreedName))
        {
            // Hayvan ilanı: "Holstein Süt İneği - Dişi - 24 Aylık - İzmir"
            parts.Add(ai.BreedName);
            if (!string.IsNullOrWhiteSpace(p.Category?.Name))
                parts.Add(p.Category.Name);
            parts.Add(GenderLabel(ai.Gender));
            if (ai.AgeMonths > 0)
                parts.Add($"{ai.AgeMonths} Aylık");
        }
        else
        {
            // Genel ürün: "John Deere Traktör Lastiği - Bursa"
            if (!string.IsNullOrWhiteSpace(p.Brand?.Name))
                parts.Add(p.Brand.Name);
            if (!string.IsNullOrWhiteSpace(p.Category?.Name))
                parts.Add(p.Category.Name);
        }

        if (!string.IsNullOrWhiteSpace(p.Location?.City))
            parts.Add(p.Location.City);

        if (parts.Count == 0)
            parts.Add("Tarımsal Ürün");

        return string.Join(" - ", parts);
    }

    private static string GenerateShortDescription(Product p, AnimalInfo? ai)
    {
        var sb = new StringBuilder();

        if (ai != null && !string.IsNullOrWhiteSpace(ai.BreedName))
        {
            sb.Append($"{ai.BreedName} cinsi");
            if (ai.AgeMonths > 0) sb.Append($", {ai.AgeMonths} aylık");
            if (ai.WeightKg > 0) sb.Append($", {ai.WeightKg:N0} kg");
            sb.Append($", {HealthLabel(ai.HealthStatus)} {GenderLabel(ai.Gender).ToLower(new CultureInfo("tr-TR"))}");
            if (!string.IsNullOrWhiteSpace(p.Category?.Name))
                sb.Append($" {p.Category.Name.ToLower(new CultureInfo("tr-TR"))}");
            sb.Append('.');
            if (ai.Purpose != AnimalPurpose.Mixed)
                sb.Append($" {PurposeLabel(ai.Purpose)} olarak uygundur.");
        }
        else
        {
            if (!string.IsNullOrWhiteSpace(p.Brand?.Name))
                sb.Append($"{p.Brand.Name} marka ");
            if (!string.IsNullOrWhiteSpace(p.Category?.Name))
                sb.Append($"{p.Category.Name.ToLower(new CultureInfo("tr-TR"))}");
            else
                sb.Append("tarımsal ürün");
            sb.Append('.');
            sb.Append($" {ConditionLabel(p.Condition)}");
            if (!string.IsNullOrWhiteSpace(p.Location?.City))
                sb.Append($", {p.Location.City}'da mevcut.");
            else
                sb.Append('.');
        }

        return sb.ToString();
    }

    private static string GenerateDescription(Product p, AnimalInfo? ai)
    {
        var sb = new StringBuilder();

        if (ai != null && !string.IsNullOrWhiteSpace(ai.BreedName))
        {
            sb.AppendLine("## Hayvan Bilgileri");
            sb.AppendLine($"- **Cins/Irk:** {ai.BreedName}");
            sb.AppendLine($"- **Cinsiyet:** {GenderLabel(ai.Gender)}");
            if (ai.AgeMonths > 0) sb.AppendLine($"- **Yaş:** {ai.AgeMonths} ay");
            if (ai.WeightKg > 0) sb.AppendLine($"- **Ağırlık:** {ai.WeightKg:N0} kg");
            if (ai.HeightCm > 0) sb.AppendLine($"- **Boy:** {ai.HeightCm:N0} cm");
            if (!string.IsNullOrWhiteSpace(ai.Color)) sb.AppendLine($"- **Renk:** {ai.Color}");
            if (ai.Purpose != AnimalPurpose.Mixed) sb.AppendLine($"- **Amaç:** {PurposeLabel(ai.Purpose)}");
            if (!string.IsNullOrWhiteSpace(ai.TagNumber)) sb.AppendLine($"- **Küpe No:** {ai.TagNumber}");
            sb.AppendLine();

            sb.AppendLine("## Sağlık Durumu");
            sb.AppendLine($"- **Genel Durum:** {HealthLabel(ai.HealthStatus)}");
            if (ai.IsPregnant)
            {
                sb.Append("- **Gebelik:** Evet");
                if (ai.ExpectedDueDate != null) sb.Append($" (tahmini doğum: {ai.ExpectedDueDate:dd.MM.yyyy})");
                sb.AppendLine();
            }
            if (ai.DailyMilkProductionLiters > 0)
                sb.AppendLine($"- **Günlük Süt Üretimi:** {ai.DailyMilkProductionLiters:N1} litre");
            if (ai.NumberOfBirths > 0)
                sb.AppendLine($"- **Doğum Sayısı:** {ai.NumberOfBirths}");
            sb.AppendLine();
        }
        else
        {
            sb.AppendLine("## Ürün Bilgileri");
            if (!string.IsNullOrWhiteSpace(p.Brand?.Name)) sb.AppendLine($"- **Marka:** {p.Brand.Name}");
            if (!string.IsNullOrWhiteSpace(p.Category?.Name)) sb.AppendLine($"- **Kategori:** {p.Category.Name}");
            sb.AppendLine($"- **Kondisyon:** {ConditionLabel(p.Condition)}");
            sb.AppendLine();
        }

        // Konum
        if (p.Location != null && !string.IsNullOrWhiteSpace(p.Location.City))
        {
            sb.AppendLine("## Konum");
            var locParts = new List<string>();
            if (!string.IsNullOrWhiteSpace(p.Location.City)) locParts.Add(p.Location.City);
            if (!string.IsNullOrWhiteSpace(p.Location.State)) locParts.Add(p.Location.State);
            locParts.Add(CountryName(p.Location.CountryCode));
            sb.AppendLine(string.Join(", ", locParts));
            sb.AppendLine();
        }

        // Fiyat
        sb.AppendLine("## Fiyat");
        sb.AppendLine($"**{p.BasePrice:N0} {p.Currency}**");
        if (p.StockQuantity > 1) sb.AppendLine($"Stok: {p.StockQuantity} adet");
        if (p.IsShippingAvailable) sb.AppendLine("Kargo imkanı mevcuttur.");
        sb.AppendLine();

        // Satıcı
        if (p.Seller != null && !string.IsNullOrWhiteSpace(p.Seller.BusinessName))
        {
            sb.AppendLine("---");
            sb.AppendLine($"*{p.Seller.BusinessName} tarafından yayınlanmıştır.*");
        }

        return sb.ToString().TrimEnd();
    }

    // ───────────── Helpers ─────────────

    private static string GenderLabel(AnimalGender g) => g switch
    {
        AnimalGender.Male => "Erkek",
        AnimalGender.Female => "Dişi",
        AnimalGender.Castrated => "İğdiş",
        _ => "Belirtilmemiş"
    };

    private static string HealthLabel(HealthStatus h) => h switch
    {
        HealthStatus.Healthy => "sağlıklı",
        HealthStatus.UnderTreatment => "tedavi altında",
        HealthStatus.Quarantine => "karantinada",
        HealthStatus.Recovering => "iyileşme sürecinde",
        _ => "belirtilmemiş"
    };

    private static string PurposeLabel(AnimalPurpose p) => p switch
    {
        AnimalPurpose.Breeding => "Damızlık",
        AnimalPurpose.Meat => "Besicilik",
        AnimalPurpose.Dairy => "Süt üretimi",
        AnimalPurpose.EggProduction => "Yumurta üretimi",
        AnimalPurpose.Work => "İş hayvanı",
        AnimalPurpose.Pet => "Evcil hayvan",
        AnimalPurpose.Show => "Gösteri/Yarış",
        _ => "Genel amaçlı"
    };

    private static string ConditionLabel(ProductCondition c) => c switch
    {
        ProductCondition.New => "Sıfır/Yeni",
        ProductCondition.Used => "İkinci el",
        ProductCondition.Refurbished => "Yenilenmiş",
        ProductCondition.ForParts => "Yedek parça için",
        _ => ""
    };

    private static string CountryName(string? code) => code?.ToUpperInvariant() switch
    {
        "TR" => "Türkiye",
        "AZ" => "Azerbaycan",
        "KZ" => "Kazakistan",
        "UZ" => "Özbekistan",
        "DE" => "Almanya",
        "NL" => "Hollanda",
        "US" => "Amerika",
        "GB" => "İngiltere",
        "FR" => "Fransa",
        "IR" => "İran",
        "IQ" => "Irak",
        "SA" => "Suudi Arabistan",
        "EG" => "Mısır",
        "RU" => "Rusya",
        _ => code ?? "—"
    };

    private static string Slugify(string text)
    {
        if (string.IsNullOrWhiteSpace(text)) return "ilan";

        // Turkish chars
        var slug = text
            .Replace("ş", "s").Replace("Ş", "s")
            .Replace("ç", "c").Replace("Ç", "c")
            .Replace("ğ", "g").Replace("Ğ", "g")
            .Replace("ü", "u").Replace("Ü", "u")
            .Replace("ö", "o").Replace("Ö", "o")
            .Replace("ı", "i").Replace("İ", "i");

        slug = slug.Normalize(NormalizationForm.FormD);
        slug = Regex.Replace(slug, @"[\u0300-\u036f]", ""); // diacritics
        slug = slug.ToLowerInvariant();
        slug = Regex.Replace(slug, @"[^a-z0-9\s-]", "");
        slug = Regex.Replace(slug, @"[\s-]+", "-").Trim('-');

        return slug.Length > 80 ? slug[..80].TrimEnd('-') : slug;
    }

    private static string Truncate(string? s, int max)
    {
        if (string.IsNullOrWhiteSpace(s)) return "—";
        return s.Length <= max ? s : s[..max] + "…";
    }
}
