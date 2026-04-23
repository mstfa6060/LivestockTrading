using System;

namespace LivestockTrading.Domain.Entities;

/// <summary>
/// Veteriner ürünleri için özel bilgiler
/// İlaçlar, aşılar, tıbbi ekipman
/// </summary>
public class VeterinaryInfo : BaseEntity
{
    public Guid ProductId { get; set; }
    public Product Product { get; set; }
    
    /// <summary>Veteriner ürün tipi</summary>
    public VeterinaryProductType Type { get; set; }
    /// <summary>Terapötik kategori</summary>
    public string TherapeuticCategory { get; set; }
    /// <summary>Hedef türler JSON: ["Sığır","At","Köpek"]</summary>
    public string TargetSpecies { get; set; }
    /// <summary>Endikasyonlar (Ne için kullanılır)</summary>
    public string Indications { get; set; }
    /// <summary>Aktif maddeler JSON</summary>
    public string ActiveIngredients { get; set; }
    /// <summary>Doz gücü</summary>
    public string Strength { get; set; }
    /// <summary>Uygulama yolu</summary>
    public AdministrationRoute Route { get; set; }
    /// <summary>Dozaj talimatları</summary>
    public string DosageInstructions { get; set; }
    /// <summary>Uygulama sıklığı</summary>
    public string Frequency { get; set; }
    /// <summary>Tedavi süresi (gün)</summary>
    public int? TreatmentDurationDays { get; set; }
    /// <summary>Reçete gerekiyor mu?</summary>
    public bool RequiresPrescription { get; set; }
    /// <summary>Kayıt numarası</summary>
    public string RegistrationNumber { get; set; }
    /// <summary>Onay kurumu (FDA, EMA vb.)</summary>
    public string ApprovalAgency { get; set; }
    /// <summary>Onay tarihi</summary>
    public DateTime? ApprovalDate { get; set; }
    /// <summary>Kontrendikasyonlar</summary>
    public string Contraindications { get; set; }
    /// <summary>Yan etkiler</summary>
    public string SideEffects { get; set; }
    /// <summary>Uyarılar</summary>
    public string Warnings { get; set; }
    /// <summary>Önlemler</summary>
    public string Precautions { get; set; }
    /// <summary>Et için bekleme süresi (gün)</summary>
    public int? MeatWithdrawalDays { get; set; }
    /// <summary>Süt için bekleme süresi (gün)</summary>
    public int? MilkWithdrawalDays { get; set; }
    /// <summary>Yumurta için bekleme süresi (gün)</summary>
    public int? EggWithdrawalDays { get; set; }
    /// <summary>Depolama talimatları</summary>
    public string StorageInstructions { get; set; }
    /// <summary>Depolama sıcaklığı</summary>
    public string StorageTemperature { get; set; }
    /// <summary>Raf ömrü (ay)</summary>
    public int? ShelfLifeMonths { get; set; }
    /// <summary>Üretim tarihi</summary>
    public DateTime? ManufactureDate { get; set; }
    /// <summary>Son kullanma tarihi</summary>
    public DateTime? ExpiryDate { get; set; }
    /// <summary>Parti numarası</summary>
    public string BatchNumber { get; set; }
    /// <summary>Soğuk zincir gerekiyor mu?</summary>
    public bool RequiresColdChain { get; set; }
    /// <summary>Teknik özellikler JSON (tıbbi ekipman için)</summary>
    public string TechnicalSpecifications { get; set; }
    /// <summary>Bakım talimatları</summary>
    public string MaintenanceInstructions { get; set; }
    /// <summary>Sertifikalar JSON: ["GMP","ISO"]</summary>
    public string Certifications { get; set; }
}

public enum VeterinaryProductType
{
    Antibiotic = 0,
    Antiparasitic = 1,
    Vaccine = 2,
    AntiInflammatory = 3,
    Analgesic = 4,
    Vitamin = 5,
    Hormone = 6,
    Anesthetic = 7,
    Antiseptic = 8,
    Bandage = 9,
    MedicalEquipment = 10,
    DiagnosticKit = 11,
    Disinfectant = 12,
    WoundCare = 13,
    Other = 99
}

public enum AdministrationRoute
{
    Oral = 0,
    Injection = 1,
    Topical = 2,
    Intravenous = 3,
    Intramuscular = 4,
    Subcutaneous = 5,
    Inhalation = 6,
    Ophthalmic = 7,
    Otic = 8,
    Rectal = 9,
    Other = 99
}
