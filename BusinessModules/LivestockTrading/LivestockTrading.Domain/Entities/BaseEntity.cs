using System;

namespace LivestockTrading.Domain.Entities;

/// <summary>
/// Tüm domain entity'leri için temel sınıf
/// </summary>
public abstract class BaseEntity
{
    /// <summary>Benzersiz kimlik</summary>
    public Guid Id { get; set; }
    
    /// <summary>Oluşturulma tarihi</summary>
    public DateTime CreatedAt { get; set; }
    
    /// <summary>Güncellenme tarihi</summary>
    public DateTime? UpdatedAt { get; set; }
    
    /// <summary>Silinmiş mi?</summary>
    public bool IsDeleted { get; set; }
    
    /// <summary>Silinme tarihi</summary>
    public DateTime? DeletedAt { get; set; }
    
    protected BaseEntity()
    {
        Id = Guid.NewGuid();
        CreatedAt = DateTime.UtcNow;
        IsDeleted = false;
    }
}
