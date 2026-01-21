using System;

namespace LivestockTrading.Domain.Entities;

/// <summary>
/// Yem ve beslenme ürünleri için özel bilgiler
/// </summary>
public class FeedInfo : BaseEntity
{
    public Guid ProductId { get; set; }
    public Product Product { get; set; }
    
    /// <summary>Yem tipi</summary>
    public FeedType Type { get; set; }
    /// <summary>Hedef hayvan JSON: ["Sığır","Koyun","Kanatlı"]</summary>
    public string TargetAnimal { get; set; }
    /// <summary>Hedef yaş (Yetişkin, Genç, Yavru)</summary>
    public string TargetAge { get; set; }
    /// <summary>Protein oranı (%)</summary>
    public decimal? ProteinPercentage { get; set; }
    /// <summary>Yağ oranı (%)</summary>
    public decimal? FatPercentage { get; set; }
    /// <summary>Lif oranı (%)</summary>
    public decimal? FiberPercentage { get; set; }
    /// <summary>Nem oranı (%)</summary>
    public decimal? MoisturePercentage { get; set; }
    /// <summary>Kül oranı (%)</summary>
    public decimal? AshPercentage { get; set; }
    /// <summary>Kalsiyum oranı (%)</summary>
    public decimal? CalciumPercentage { get; set; }
    /// <summary>Fosfor oranı (%)</summary>
    public decimal? PhosphorusPercentage { get; set; }
    /// <summary>Enerji (kcal/kg)</summary>
    public decimal? EnergyKcalPerKg { get; set; }
    /// <summary>Metabolik enerji (MJ/kg)</summary>
    public decimal? MetabolizableEnergyMJPerKg { get; set; }
    /// <summary>Vitamin ve mineraller detaylı JSON</summary>
    public string VitaminsAndMinerals { get; set; }
    /// <summary>İçerik listesi (yüzdeye göre sıralı)</summary>
    public string IngredientsList { get; set; }
    /// <summary>Katkı maddeleri listesi</summary>
    public string AdditivesList { get; set; }
    /// <summary>Yem formu</summary>
    public FeedForm Form { get; set; }
    /// <summary>Partikül boyutu</summary>
    public string ParticleSize { get; set; }
    /// <summary>Organik mi?</summary>
    public bool IsOrganic { get; set; }
    /// <summary>GMO içermiyor mu?</summary>
    public bool IsGMOFree { get; set; }
    /// <summary>İlaçlı yem mi?</summary>
    public bool IsMedicatedFeed { get; set; }
    /// <summary>Besleme talimatları</summary>
    public string FeedingInstructions { get; set; }
    /// <summary>Önerilen günlük miktar</summary>
    public string RecommendedDailyAmount { get; set; }
    /// <summary>Besleme sıklığı</summary>
    public string FeedingFrequency { get; set; }
    /// <summary>Depolama talimatları</summary>
    public string StorageInstructions { get; set; }
    /// <summary>Raf ömrü (ay)</summary>
    public int? ShelfLifeMonths { get; set; }
    /// <summary>Üretim tarihi</summary>
    public DateTime? ManufactureDate { get; set; }
    /// <summary>Son kullanma tarihi</summary>
    public DateTime? ExpiryDate { get; set; }
    /// <summary>Parti numarası</summary>
    public string BatchNumber { get; set; }
    /// <summary>Kalite test sonuçları JSON</summary>
    public string QualityTestResults { get; set; }
    /// <summary>Sertifikalar JSON: ["GMP","ISO","Organik"]</summary>
    public string Certifications { get; set; }
    /// <summary>Uyarılar</summary>
    public string Warnings { get; set; }
    /// <summary>Kontrendikasyonlar</summary>
    public string Contraindications { get; set; }
}

public enum FeedType
{
    CompleteFeed = 0,
    Supplement = 1,
    Concentrate = 2,
    Forage = 3,
    Hay = 4,
    Silage = 5,
    Grain = 6,
    MineralBlock = 7,
    VitaminSupplement = 8,
    ProteinSupplement = 9,
    Premix = 10,
    MilkReplacer = 11,
    Other = 99
}

public enum FeedForm
{
    Pellets = 0,
    Crumbles = 1,
    Mash = 2,
    Granules = 3,
    Powder = 4,
    Liquid = 5,
    Block = 6,
    Cubes = 7,
    Bales = 8,
    Other = 99
}
