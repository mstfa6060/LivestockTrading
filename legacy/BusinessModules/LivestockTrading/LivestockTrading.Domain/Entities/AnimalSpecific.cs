using System;
using System.Collections.Generic;

namespace LivestockTrading.Domain.Entities;

/// <summary>
/// Hayvanlara özel detaylı bilgiler
/// Canlı hayvan ürünleri için genişletilmiş özellikler
/// </summary>
public class AnimalInfo : BaseEntity
{
    public Guid ProductId { get; set; }
    public Product Product { get; set; }
    
    /// <summary>Irk/Cins adı</summary>
    public string BreedName { get; set; }
    /// <summary>Cinsiyet</summary>
    public AnimalGender Gender { get; set; }
    /// <summary>Doğum tarihi</summary>
    public DateTime? DateOfBirth { get; set; }
    /// <summary>Yaş (ay)</summary>
    public int? AgeMonths { get; set; }
    /// <summary>Ağırlık (kg)</summary>
    public double? WeightKg { get; set; }
    /// <summary>Boy (cm)</summary>
    public double? HeightCm { get; set; }
    /// <summary>Renk</summary>
    public string Color { get; set; }
    /// <summary>İşaretler/Özellikler</summary>
    public string Markings { get; set; }
    /// <summary>Küpe numarası</summary>
    public string TagNumber { get; set; }
    /// <summary>Mikroçip numarası</summary>
    public string MicrochipNumber { get; set; }
    /// <summary>Pasaport numarası</summary>
    public string PassportNumber { get; set; }
    /// <summary>Kayıt numarası</summary>
    public string RegistrationNumber { get; set; }
    /// <summary>Sağlık durumu</summary>
    public HealthStatus HealthStatus { get; set; }
    /// <summary>Son sağlık kontrolü tarihi</summary>
    public DateTime? LastHealthCheckDate { get; set; }
    /// <summary>Gebe mi?</summary>
    public bool IsPregnant { get; set; }
    /// <summary>Tahmini doğum tarihi</summary>
    public DateTime? ExpectedDueDate { get; set; }
    /// <summary>Doğum sayısı</summary>
    public int? NumberOfBirths { get; set; }
    /// <summary>Günlük süt üretimi (litre)</summary>
    public double? DailyMilkProductionLiters { get; set; }
    /// <summary>Ortalama günlük yumurta üretimi</summary>
    public double? AverageDailyEggProduction { get; set; }
    /// <summary>Baba bilgileri</summary>
    public string SireDetails { get; set; }
    /// <summary>Anne bilgileri</summary>
    public string DamDetails { get; set; }
    /// <summary>Kullanım amacı</summary>
    public AnimalPurpose Purpose { get; set; }
    
    public ICollection<HealthRecord> HealthRecords { get; set; }
    public ICollection<Vaccination> Vaccinations { get; set; }
    
    public AnimalInfo()
    {
        HealthRecords = new HashSet<HealthRecord>();
        Vaccinations = new HashSet<Vaccination>();
        HealthStatus = HealthStatus.Healthy;
    }
}

public enum AnimalGender
{
    Male = 0,
    Female = 1,
    Castrated = 2,
    Unknown = 3
}

public enum HealthStatus
{
    Healthy = 0,
    UnderTreatment = 1,
    Quarantine = 2,
    Recovering = 3,
    Unknown = 4
}

public enum AnimalPurpose
{
    Breeding = 0,
    Meat = 1,
    Dairy = 2,
    EggProduction = 3,
    Work = 4,
    Pet = 5,
    Show = 6,
    Mixed = 7
}

/// <summary>
/// Hayvan sağlık kayıtları
/// </summary>
public class HealthRecord : BaseEntity
{
    public Guid AnimalInfoId { get; set; }
    public AnimalInfo AnimalInfo { get; set; }
    
    /// <summary>Kayıt tarihi</summary>
    public DateTime RecordDate { get; set; }
    /// <summary>Kayıt tipi (Kontrol, Tedavi, Ameliyat vb.)</summary>
    public string RecordType { get; set; }
    /// <summary>Veteriner adı</summary>
    public string VeterinarianName { get; set; }
    /// <summary>Veteriner lisans no</summary>
    public string VeterinarianLicense { get; set; }
    /// <summary>Klinik adı</summary>
    public string ClinicName { get; set; }
    /// <summary>Teşhis</summary>
    public string Diagnosis { get; set; }
    /// <summary>Tedavi</summary>
    public string Treatment { get; set; }
    /// <summary>İlaçlar</summary>
    public string Medications { get; set; }
    /// <summary>Notlar</summary>
    public string Notes { get; set; }
    /// <summary>Takip randevu tarihi</summary>
    public DateTime? FollowUpDate { get; set; }
    /// <summary>Veteriner raporu URL (FileProvider'dan)</summary>
    public string DocumentUrl { get; set; }
    
    public HealthRecord()
    {
        RecordDate = DateTime.UtcNow;
    }
}

/// <summary>
/// Aşı kayıtları
/// </summary>
public class Vaccination : BaseEntity
{
    public Guid AnimalInfoId { get; set; }
    public AnimalInfo AnimalInfo { get; set; }
    
    /// <summary>Aşı adı</summary>
    public string VaccineName { get; set; }
    /// <summary>Aşı tipi</summary>
    public string VaccineType { get; set; }
    /// <summary>Parti numarası</summary>
    public string BatchNumber { get; set; }
    /// <summary>Aşı tarihi</summary>
    public DateTime VaccinationDate { get; set; }
    /// <summary>Sonraki aşı tarihi</summary>
    public DateTime? NextDueDate { get; set; }
    /// <summary>Veteriner adı</summary>
    public string VeterinarianName { get; set; }
    /// <summary>Veteriner lisans no</summary>
    public string VeterinarianLicense { get; set; }
    /// <summary>Notlar</summary>
    public string Notes { get; set; }
    /// <summary>Aşı sertifikası URL (FileProvider'dan)</summary>
    public string CertificateUrl { get; set; }
    
    public Vaccination()
    {
        VaccinationDate = DateTime.UtcNow;
    }
}
