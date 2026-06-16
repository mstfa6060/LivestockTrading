using Livestock.Domain.Enums;

namespace Livestock.Domain.Entities;

public class Offer : BaseEntity
{
    public Guid ProductId { get; set; }
    public Guid BuyerUserId { get; set; }
    public Guid SellerId { get; set; }
    public decimal OfferedPrice { get; set; }
    public string CurrencyCode { get; set; } = "USD";
    public int Quantity { get; set; }
    public string? Note { get; set; }
    public OfferStatus Status { get; set; } = OfferStatus.Pending;
    public decimal? CounterPrice { get; set; }
    public string? CounterNote { get; set; }
    public DateTime? ExpiresAt { get; set; }

    public Product Product { get; set; } = null!;
    public Deal? Deal { get; set; }
}
