using System;
using System.Collections.Generic;

namespace LivestockTrading.Domain.Entities;

/// <summary>
/// Tarım makineleri ve ekipmanlar için özel bilgiler
/// Traktör, biçerdöver, pulluk vb.
/// </summary>
public class MachineryInfo : BaseEntity
{
    public Guid ProductId { get; set; }
    public Product Product { get; set; }
    
    /// <summary>Makine tipi</summary>
    public MachineryType Type { get; set; }
    /// <summary>Model</summary>
    public string Model { get; set; }
    /// <summary>Üretim yılı</summary>
    public int? YearOfManufacture { get; set; }
    /// <summary>Seri numarası</summary>
    public string SerialNumber { get; set; }
    /// <summary>Güç kaynağı (Dizel, Elektrik, Manuel, Benzin, Hibrit)</summary>
    public string PowerSource { get; set; }
    /// <summary>Güç (HP)</summary>
    public decimal? PowerHp { get; set; }
    /// <summary>Güç (kW)</summary>
    public decimal? PowerKw { get; set; }
    /// <summary>Motor kapasitesi</summary>
    public string EngineCapacity { get; set; }
    /// <summary>Uzunluk (cm)</summary>
    public decimal? LengthCm { get; set; }
    /// <summary>Genişlik (cm)</summary>
    public decimal? WidthCm { get; set; }
    /// <summary>Yükseklik (cm)</summary>
    public decimal? HeightCm { get; set; }
    /// <summary>Ağırlık (kg)</summary>
    public decimal? WeightKg { get; set; }
    /// <summary>Çalışma genişliği (cm)</summary>
    public decimal? WorkingWidthCm { get; set; }
    /// <summary>Kapasite (litre)</summary>
    public decimal? CapacityLiters { get; set; }
    /// <summary>Yük kapasitesi (kg)</summary>
    public decimal? LoadCapacityKg { get; set; }
    /// <summary>Hız (km/s)</summary>
    public decimal? SpeedKmh { get; set; }
    /// <summary>Kullanım saati</summary>
    public int? HoursUsed { get; set; }
    /// <summary>Son servis tarihi</summary>
    public DateTime? LastServiceDate { get; set; }
    /// <summary>Servis geçmişi JSON</summary>
    public string ServiceHistory { get; set; }
    /// <summary>Dahil edilen aksesuarlar JSON</summary>
    public string IncludedAttachments { get; set; }
    /// <summary>Uyumlu aksesuarlar JSON</summary>
    public string CompatibleAttachments { get; set; }
    /// <summary>Garantisi var mı?</summary>
    public bool HasWarranty { get; set; }
    /// <summary>Garanti bitiş tarihi</summary>
    public DateTime? WarrantyExpiryDate { get; set; }
    /// <summary>Garanti detayları</summary>
    public string WarrantyDetails { get; set; }
    /// <summary>Sertifikalar JSON: ["CE","ISO"]</summary>
    public string Certifications { get; set; }
    /// <summary>Güvenlik özellikleri JSON</summary>
    public string SafetyFeatures { get; set; }
    
    public MachineryInfo()
    {
        HasWarranty = false;
    }
}

public enum MachineryType
{
    Tractor = 0,
    Harvester = 1,
    Plow = 2,
    Seeder = 3,
    Sprayer = 4,
    Cultivator = 5,
    Baler = 6,
    Mower = 7,
    IrrigationSystem = 8,
    Generator = 9,
    Pump = 10,
    Trailer = 11,
    HandTool = 12,
    PowerTool = 13,
    ProcessingEquipment = 14,
    StorageEquipment = 15,
    Other = 99
}
