using Livestock.Domain.Enums;

namespace Livestock.Domain.Entities;

public class IAPTransaction : BaseEntity
{
    public Guid UserId { get; set; }
    public IAPTransactionType TransactionType { get; set; }
    public IAPTransactionStatus Status { get; set; } = IAPTransactionStatus.Pending;
    public SubscriptionPlatform Platform { get; set; }
    public string? ExternalTransactionId { get; set; }
    public string? ProductId { get; set; }
    public string? ReceiptData { get; set; }
    public decimal Amount { get; set; }
    public string CurrencyCode { get; set; } = "USD";
    public Guid? SubscriptionId { get; set; }
    public Guid? BoostId { get; set; }
    public string? FailureReason { get; set; }
    public DateTime? ValidatedAt { get; set; }
}
