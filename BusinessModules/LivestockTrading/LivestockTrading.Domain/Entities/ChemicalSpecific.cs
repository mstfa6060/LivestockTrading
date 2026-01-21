using System;

namespace LivestockTrading.Domain.Entities;

/// <summary>
/// Tarımsal kimyasallar ve gübreler için özel bilgiler
/// İlaç, gübre, herbisit, fungisit vb.
/// </summary>
public class ChemicalInfo : BaseEntity
{
    public Guid ProductId { get; set; }
    public Product Product { get; set; }
    
    /// <summary>Kimyasal tipi</summary>
    public ChemicalType Type { get; set; }
    /// <summary>Alt tip</summary>
    public string SubType { get; set; }
    /// <summary>Aktif bileşenler JSON: [{"name":"Glyphosate","percentage":41}]</summary>
    public string ActiveIngredients { get; set; }
    /// <summary>İnert bileşenler</summary>
    public string InertIngredients { get; set; }
    /// <summary>Kimyasal formül</summary>
    public string ChemicalFormula { get; set; }
    /// <summary>Kayıt numarası</summary>
    public string RegistrationNumber { get; set; }
    /// <summary>Onay kurumu (EPA, EU, FAO vb.)</summary>
    public string ApprovalAgency { get; set; }
    /// <summary>Kayıt tarihi</summary>
    public DateTime? RegistrationDate { get; set; }
    /// <summary>Son kullanma tarihi</summary>
    public DateTime? ExpiryDate { get; set; }
    /// <summary>Uygulama yöntemi (Sprey, Granül, Sıvı vb.)</summary>
    public string ApplicationMethod { get; set; }
    /// <summary>Hedef zararlılar JSON</summary>
    public string TargetPests { get; set; }
    /// <summary>Hedef bitkiler JSON</summary>
    public string TargetCrops { get; set; }
    /// <summary>Dozaj talimatları</summary>
    public string DosageInstructions { get; set; }
    /// <summary>Toksisite seviyesi</summary>
    public ToxicityLevel ToxicityLevel { get; set; }
    /// <summary>Güvenlik talimatları</summary>
    public string SafetyInstructions { get; set; }
    /// <summary>İlk yardım talimatları</summary>
    public string FirstAidInstructions { get; set; }
    /// <summary>Alana tekrar giriş süresi (saat)</summary>
    public int? ReEntryIntervalHours { get; set; }
    /// <summary>Hasat öncesi bekleme süresi (gün)</summary>
    public int? PreHarvestIntervalDays { get; set; }
    /// <summary>Organik mi?</summary>
    public bool IsOrganic { get; set; }
    /// <summary>Biyolojik olarak parçalanabilir mi?</summary>
    public bool IsBiodegradable { get; set; }
    /// <summary>Çevresel etki</summary>
    public string EnvironmentalImpact { get; set; }
    /// <summary>Depolama talimatları</summary>
    public string StorageInstructions { get; set; }
    /// <summary>Depolama sıcaklığı</summary>
    public string StorageTemperature { get; set; }
    /// <summary>Raf ömrü (ay)</summary>
    public int? ShelfLifeMonths { get; set; }
    /// <summary>Sertifikalar JSON: ["OMRI Listed","USDA Organic"]</summary>
    public string Certifications { get; set; }
}

public enum ChemicalType
{
    Fertilizer = 0,
    Pesticide = 1,
    Herbicide = 2,
    Fungicide = 3,
    Insecticide = 4,
    Rodenticide = 5,
    PlantGrowthRegulator = 6,
    SoilConditioner = 7,
    Disinfectant = 8,
    Other = 99
}

public enum ToxicityLevel
{
    NonToxic = 0,
    SlightlyToxic = 1,
    ModeratelyToxic = 2,
    HighlyToxic = 3,
    ExtremelyToxic = 4
}
