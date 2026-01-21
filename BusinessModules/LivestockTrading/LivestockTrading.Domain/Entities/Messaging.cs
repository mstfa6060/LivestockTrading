using System;
using System.Collections.Generic;

namespace LivestockTrading.Domain.Entities;

/// <summary>
/// Kullanıcılar arası konuşma
/// </summary>
public class Conversation : BaseEntity
{
    /// <summary>Katılımcı 1 ID</summary>
    public Guid ParticipantUserId1 { get; set; }
    /// <summary>Katılımcı 2 ID</summary>
    public Guid ParticipantUserId2 { get; set; }
    /// <summary>Ürün ID (eğer ürünle ilgiliyse)</summary>
    public Guid? ProductId { get; set; }
    public Product Product { get; set; }
    /// <summary>Sipariş ID (eğer siparişle ilgiliyse)</summary>
    public Guid? OrderId { get; set; }
    /// <summary>Konu</summary>
    public string Subject { get; set; }
    /// <summary>Durum</summary>
    public ConversationStatus Status { get; set; }
    /// <summary>Son mesaj tarihi</summary>
    public DateTime LastMessageAt { get; set; }
    
    public ICollection<Message> Messages { get; set; }
    
    public Conversation()
    {
        Messages = new HashSet<Message>();
        Status = ConversationStatus.Active;
        LastMessageAt = DateTime.UtcNow;
    }
}

public enum ConversationStatus
{
    Active = 0,
    Archived = 1,
    Resolved = 2,
    Blocked = 3
}

/// <summary>
/// Tekil mesaj
/// </summary>
public class Message : BaseEntity
{
    public Guid ConversationId { get; set; }
    public Conversation Conversation { get; set; }
    /// <summary>Gönderen kullanıcı ID</summary>
    public Guid SenderUserId { get; set; }
    /// <summary>Alıcı kullanıcı ID</summary>
    public Guid RecipientUserId { get; set; }
    /// <summary>Mesaj içeriği</summary>
    public string Content { get; set; }
    /// <summary>Okundu mu?</summary>
    public bool IsRead { get; set; }
    /// <summary>Okunma tarihi</summary>
    public DateTime? ReadAt { get; set; }
    /// <summary>Ek dosya URL'leri JSON (FileProvider'dan)</summary>
    public string AttachmentUrls { get; set; }
    /// <summary>Gönderilme tarihi</summary>
    public DateTime SentAt { get; set; }
    
    public Message()
    {
        IsRead = false;
        SentAt = DateTime.UtcNow;
    }
}

/// <summary>
/// Alıcı-satıcı arası teklif/karşı teklif
/// </summary>
public class Offer : BaseEntity
{
    public Guid ProductId { get; set; }
    public Product Product { get; set; }
    /// <summary>Alıcı kullanıcı ID</summary>
    public Guid BuyerUserId { get; set; }
    /// <summary>Satıcı kullanıcı ID</summary>
    public Guid SellerUserId { get; set; }
    /// <summary>Teklif edilen fiyat</summary>
    public decimal OfferedPrice { get; set; }
    /// <summary>Para birimi</summary>
    public string Currency { get; set; }
    /// <summary>Miktar</summary>
    public int Quantity { get; set; }
    /// <summary>Mesaj</summary>
    public string Message { get; set; }
    /// <summary>Teklif durumu</summary>
    public OfferStatus Status { get; set; }
    /// <summary>Teklif tarihi</summary>
    public DateTime OfferDate { get; set; }
    /// <summary>Son geçerlilik tarihi</summary>
    public DateTime? ExpiryDate { get; set; }
    /// <summary>Yanıtlanma tarihi</summary>
    public DateTime? RespondedAt { get; set; }
    /// <summary>Yanıt mesajı</summary>
    public string ResponseMessage { get; set; }
    /// <summary>Hangi teklife karşı teklif (eğer varsa)</summary>
    public Guid? CounterOfferToId { get; set; }
    public Offer CounterOfferTo { get; set; }
    
    public Offer()
    {
        Status = OfferStatus.Pending;
        OfferDate = DateTime.UtcNow;
        Currency = "USD";
    }
}

public enum OfferStatus
{
    Pending = 0,
    Accepted = 1,
    Rejected = 2,
    Countered = 3,
    Expired = 4,
    Withdrawn = 5
}
