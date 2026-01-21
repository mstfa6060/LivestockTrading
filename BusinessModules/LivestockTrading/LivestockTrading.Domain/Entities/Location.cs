using System;
using System.Collections.Generic;

namespace LivestockTrading.Domain.Entities;

/// <summary>
/// Coğrafi konum/Adres bilgisi
/// Ürünler, çiftlikler, satıcılar, teslimat adresleri için kullanılır
/// </summary>
public class Location : BaseEntity
{
    /// <summary>Konum adı</summary>
    public string Name { get; set; }
    /// <summary>Adres satırı 1</summary>
    public string AddressLine1 { get; set; }
    /// <summary>Adres satırı 2</summary>
    public string AddressLine2 { get; set; }
    /// <summary>Şehir</summary>
    public string City { get; set; }
    /// <summary>İl/Eyalet</summary>
    public string State { get; set; }
    /// <summary>Posta kodu</summary>
    public string PostalCode { get; set; }
    /// <summary>Ülke kodu (ISO 3166-1 alpha-2)</summary>
    public string CountryCode { get; set; }
    /// <summary>Enlem</summary>
    public decimal? Latitude { get; set; }
    /// <summary>Boylam</summary>
    public decimal? Longitude { get; set; }
    /// <summary>Telefon</summary>
    public string Phone { get; set; }
    /// <summary>E-posta</summary>
    public string Email { get; set; }
    /// <summary>Konum tipi</summary>
    public LocationType Type { get; set; }
    /// <summary>Aktif mi?</summary>
    public bool IsActive { get; set; }
    /// <summary>Kullanıcı ID (eğer kullanıcı adresiyse)</summary>
    public Guid? UserId { get; set; }
    
    public ICollection<Product> Products { get; set; }
    public ICollection<Farm> Farms { get; set; }
    public ICollection<Order> OrderShippingAddresses { get; set; }
    public ICollection<Order> OrderBillingAddresses { get; set; }
    
    public Location()
    {
        Products = new HashSet<Product>();
        Farms = new HashSet<Farm>();
        OrderShippingAddresses = new HashSet<Order>();
        OrderBillingAddresses = new HashSet<Order>();
        IsActive = true;
    }
}

public enum LocationType
{
    ProductLocation = 0,
    FarmLocation = 1,
    UserAddress = 2,
    WarehouseLocation = 3,
    ShippingAddress = 4,
    BillingAddress = 5
}
