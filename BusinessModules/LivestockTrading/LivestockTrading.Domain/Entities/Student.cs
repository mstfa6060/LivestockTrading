namespace LivestockTrading.Domain.Entities;

/// <summary>
/// Öğrenci entity'si
/// </summary>
public class Student : BaseEntity
{
    /// <summary>
    /// Öğrenci numarası
    /// </summary>
    public string StudentNumber { get; set; }

    /// <summary>
    /// Öğrencinin adı
    /// </summary>
    public string FirstName { get; set; }

    /// <summary>
    /// Öğrencinin soyadı
    /// </summary>
    public string LastName { get; set; }

    /// <summary>
    /// E-posta adresi
    /// </summary>
    public string Email { get; set; }

    /// <summary>
    /// Telefon numarası
    /// </summary>
    public string PhoneNumber { get; set; }

    /// <summary>
    /// Doğum tarihi
    /// </summary>
    public DateTime? BirthDate { get; set; }

    /// <summary>
    /// Bölüm/Sınıf
    /// </summary>
    public string Department { get; set; }

    /// <summary>
    /// Aktif mi
    /// </summary>
    public bool IsActive { get; set; } = true;
}
