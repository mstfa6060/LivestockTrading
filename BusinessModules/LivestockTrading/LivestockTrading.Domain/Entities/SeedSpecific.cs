using System;

namespace LivestockTrading.Domain.Entities;

/// <summary>
/// Tohum ve fideler için özel bilgiler
/// </summary>
public class SeedInfo : BaseEntity
{
    public Guid ProductId { get; set; }
    public Product Product { get; set; }
    
    /// <summary>Tohum tipi</summary>
    public SeedType Type { get; set; }
    /// <summary>Çeşit</summary>
    public string Variety { get; set; }
    /// <summary>Bilimsel adı</summary>
    public string ScientificName { get; set; }
    /// <summary>Yaygın adları JSON</summary>
    public string CommonNames { get; set; }
    /// <summary>Tohum boyutu</summary>
    public string SeedSize { get; set; }
    /// <summary>Tohum rengi</summary>
    public string SeedColor { get; set; }
    /// <summary>Çimlenme oranı (%)</summary>
    public decimal? GerminationRate { get; set; }
    /// <summary>Çimlenme süresi (gün)</summary>
    public int? GerminationDays { get; set; }
    /// <summary>İklim bölgeleri JSON: [7,8,9,10]</summary>
    public string ClimateZones { get; set; }
    /// <summary>Toprak tipi</summary>
    public string SoilType { get; set; }
    /// <summary>Güneş ışığı ihtiyacı (Tam Güneş, Yarı Gölge, Gölge)</summary>
    public string SunlightRequirement { get; set; }
    /// <summary>Su ihtiyacı (Düşük, Orta, Yüksek)</summary>
    public string WaterRequirement { get; set; }
    /// <summary>Ekim derinliği (cm)</summary>
    public string PlantingDepthCm { get; set; }
    /// <summary>Aralık (cm)</summary>
    public string SpacingCm { get; set; }
    /// <summary>Olgunlaşma süresi (gün)</summary>
    public int? DaysToMaturity { get; set; }
    /// <summary>Ekim mevsimi JSON: ["İlkbahar","Sonbahar"]</summary>
    public string PlantingSeason { get; set; }
    /// <summary>Hasat mevsimi</summary>
    public string HarvestSeason { get; set; }
    /// <summary>Beklenen verim</summary>
    public string ExpectedYield { get; set; }
    /// <summary>Verim birimi (kg/hektar, ton/dönüm vb.)</summary>
    public string YieldUnit { get; set; }
    /// <summary>Bitki boyu (cm)</summary>
    public string PlantHeightCm { get; set; }
    /// <summary>Bitki genişliği (cm)</summary>
    public string PlantSpreadCm { get; set; }
    /// <summary>Çiçek rengi</summary>
    public string FlowerColor { get; set; }
    /// <summary>Meyve boyutu</summary>
    public string FruitSize { get; set; }
    /// <summary>Hastalık direnci JSON</summary>
    public string DiseaseResistance { get; set; }
    /// <summary>Zararlı direnci JSON</summary>
    public string PestResistance { get; set; }
    /// <summary>Kuraklığa dayanıklı mı?</summary>
    public bool IsDroughtTolerant { get; set; }
    /// <summary>Dona dayanıklı mı?</summary>
    public bool IsFrostTolerant { get; set; }
    /// <summary>Hibrit mi?</summary>
    public bool IsHybrid { get; set; }
    /// <summary>Ata tohumu mu?</summary>
    public bool IsHeirloom { get; set; }
    /// <summary>GMO mu?</summary>
    public bool IsGMO { get; set; }
    /// <summary>Organik mi?</summary>
    public bool IsOrganic { get; set; }
    /// <summary>İşlem görmüş mü?</summary>
    public bool IsTreated { get; set; }
    /// <summary>Sertifikalar JSON: ["USDA Organic","Non-GMO"]</summary>
    public string Certifications { get; set; }
    /// <summary>Test tarihi</summary>
    public DateTime? TestDate { get; set; }
    /// <summary>Parti numarası</summary>
    public string LotNumber { get; set; }
    /// <summary>Paketleme tarihi</summary>
    public DateTime? PackageDate { get; set; }
    /// <summary>Son kullanma tarihi</summary>
    public DateTime? ExpiryDate { get; set; }
    /// <summary>Depolama talimatları</summary>
    public string StorageInstructions { get; set; }
}

public enum SeedType
{
    VegetableSeeds = 0,
    FruitSeeds = 1,
    GrainSeeds = 2,
    HerbSeeds = 3,
    FlowerSeeds = 4,
    GrassSeed = 5,
    TreeSeedlings = 6,
    Transplants = 7,
    Bulbs = 8,
    Tubers = 9,
    CoverCropSeeds = 10,
    Other = 99
}
