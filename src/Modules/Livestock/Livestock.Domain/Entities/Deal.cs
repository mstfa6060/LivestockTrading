using Livestock.Domain.Enums;

namespace Livestock.Domain.Entities;

public class Deal : BaseEntity
{
    public Guid OfferId { get; set; }
    public Guid ProductId { get; set; }
    public Guid BuyerUserId { get; set; }
    public Guid SellerId { get; set; }
    public decimal AgreePrice { get; set; }
    public string CurrencyCode { get; set; } = "USD";
    public int Quantity { get; set; }
    public DealStatus Status { get; set; } = DealStatus.Agreed;
    public DeliveryMethod DeliveryMethod { get; set; } = DeliveryMethod.ToBeDecided;
    public string? Notes { get; set; }
    public string? CancellationReason { get; set; }
    public DateTime? CompletedAt { get; set; }
    public DateTime? CancelledAt { get; set; }

    public Offer Offer { get; set; } = null!;
    public Product Product { get; set; } = null!;
}
