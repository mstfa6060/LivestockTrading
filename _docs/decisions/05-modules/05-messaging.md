# Karar 5 / Messaging Modülü

**Status:** FINAL
**Wave:** 6 (parallel with Marketplace + Notifications)

## İlişkili Kararlar

- **Üst:** [Karar 2](../02-modules-list.md), [Karar 3a — AR](../03-domain-patterns.md)
- **Patch:** [Patch 1.1 — Identity v2](../05-patch.md) — UserPreferences `ReadReceiptsEnabled` + `TypingIndicatorEnabled` field
- **Frontend:** `frontend-api-inventory.md` Messaging (14 endpoint — conversation, message, typing, attachments)
- **Karar 5/Messaging:** SignalR separate hub `/messaging/hub`; typing pure WS no persist; read receipt opt-out server-enforced; auto-translate on-demand endpoint; system messages 12 template

---

## 1. Modülün Rolü ve Sınırları

### Sahip

| Konsept | Sahiplik |
|---|---|
| Conversation AR (1-1 thread Faz 1; group chat Faz 2) | Messaging |
| Message lifecycle (text/image/offer-link/attachment/system) | Messaging |
| Read receipts (per user-message) | Messaging |
| Typing indicator (ephemeral, no persist, no event bus) | Messaging |
| Conversation participant state (archive/pin/last-read) | Messaging |
| User-to-user block | Messaging (junction entity) |
| Message report → admin moderation queue | Messaging |
| SignalR `/messaging/hub` realtime push | Messaging |

### Sahip Olmayan

| Konsept | Sahibi |
|---|---|
| Offer/Deal lifecycle (system messages tetikleyici) | Marketplace |
| User identity / push token | Identity |
| Notification fan-out (cross-channel) | Notifications |
| AI translation actual call | Listings worker veya Shared/AI Faz 2 |
| Attachment physical storage | Shared IFileStorage → MinIO |

---

## 2. Aggregate Root

### `Conversation` AR

```csharp
public class Conversation
{
    public Guid Id;
    public Guid InitiatorUserId, RecipientUserId;                // 1-1 thread Faz 1
    
    public Guid? ContextListingId, ContextOfferId, ContextDealId;
    
    public ConversationStatus Status;                            // Active | Closed
    public DateTimeOffset CreatedAt, UpdatedAt;
    
    // Denormalize fast list rendering
    public DateTimeOffset? LastMessageAt;
    public Guid? LastMessageId;
    public string? LastMessagePreview;                            // 100 char
    public MessageType? LastMessageType;
    
    private readonly List<ConversationParticipant> _participants = new();
    private readonly List<Message> _messages = new();             // logical AR child; physically paginated
    
    public static Conversation Start(
        Guid initiatorUserId, Guid recipientUserId,
        Guid? listingId, Guid? offerId, Guid? dealId)
    {
        if (initiatorUserId == recipientUserId)
            throw new DomainException("Kendi kendine konuşma yasak");
        
        var convo = new Conversation
        {
            Id = Guid.CreateVersion7(),
            InitiatorUserId = initiatorUserId, RecipientUserId = recipientUserId,
            ContextListingId = listingId, ContextOfferId = offerId, ContextDealId = dealId,
            Status = ConversationStatus.Active,
        };
        convo._participants.Add(new ConversationParticipant(convo.Id, initiatorUserId));
        convo._participants.Add(new ConversationParticipant(convo.Id, recipientUserId));
        return convo;
        // Public event: ConversationStarted
    }
    
    public Message PostMessage(
        Guid senderUserId, MessageType type, string? content,
        string? attachmentUrl, string? attachmentMime, long? attachmentSizeBytes,
        Guid? relatedOfferId, string? systemKey, string? systemContextJson)
    {
        if (Status != ConversationStatus.Active) throw new DomainException();
        if (type != MessageType.System && !_participants.Any(p => p.UserId == senderUserId))
            throw new DomainException("Sender not a participant");
        
        var message = Message.Create(Id, senderUserId, type, content, /* ... */);
        
        // Denormalize
        LastMessageId = message.Id;
        LastMessageAt = message.CreatedAt;
        LastMessageType = type;
        LastMessagePreview = type == MessageType.Text 
            ? (content ?? "").Substring(0, Math.Min(100, content?.Length ?? 0))
            : null;
        
        var recipientParticipant = _participants.First(p => p.UserId != senderUserId);
        recipientParticipant.IncrementUnread();
        
        return message;
        // Public event: MessageSent (yüksek freq, content payload'da YOK — privacy)
    }
    
    public MessageReadReceipt MarkMessageRead(Guid readerUserId, Guid messageId)
    {
        var participant = _participants.FirstOrDefault(p => p.UserId == readerUserId);
        if (participant is null) throw new DomainException("Reader not a participant");
        
        var receipt = new MessageReadReceipt(messageId, readerUserId);
        participant.UpdateLastRead(messageId);
        participant.ResetUnread();
        return receipt;
        // Public event: MessageRead (opt-out filtered — Identity prefs check)
    }
    
    public void MarkAllRead(Guid readerUserId) { ... }
    
    public void Archive(Guid userId) { /* participant.Archive() */ }
    public void Unarchive(Guid userId) { ... }
    public void Pin(Guid userId) { ... }
    public void Unpin(Guid userId) { ... }
    public void Block(Guid blockerUserId) { /* UserBlock entity ayrı handler */ }
}

public enum ConversationStatus { Active = 1, Closed = 2 }
```

---

## 3. Child Entities

### `ConversationParticipant`

```csharp
public class ConversationParticipant
{
    public Guid Id, ConversationId, UserId;
    public bool IsArchived, IsPinned;
    public DateTimeOffset? PinnedAt, ArchivedAt;
    public Guid? LastReadMessageId;
    public DateTimeOffset? LastReadAt;
    public int UnreadCount;
    public DateTimeOffset JoinedAt;
    
    public void Archive() { ... }
    public void Pin() { ... }
    public void UpdateLastRead(Guid messageId) { ... }
    public void IncrementUnread() { ... }
    public void ResetUnread() { ... }
}
```

### `Message`

```csharp
public class Message
{
    public Guid Id, ConversationId;
    public Guid SenderUserId;                                    // System message için sentinel Guid
    public MessageType Type;
    public string? Content;                                      // Text type için
    
    // Attachment
    public string? AttachmentUrl, AttachmentMimeType, AttachmentFileName;
    public long? AttachmentSizeBytes;
    
    // OfferLink
    public Guid? RelatedOfferId;
    
    // System
    public string? SystemKey;                                    // "deal.payment_confirmed"
    public string? SystemContextJson;
    
    // Lifecycle
    public bool IsDeleted;                                       // soft delete (sender)
    public DateTimeOffset? DeletedAt;
    public bool IsEdited;
    public DateTimeOffset? EditedAt;
    public DateTimeOffset CreatedAt;
    
    public static Message Create(...) { ... }
    public static Message CreateSystem(Guid conversationId, string systemKey, object context) { ... }
    
    public void Edit(string newContent)
    {
        if (Type != MessageType.Text) throw new DomainException("Only text editable");
        if ((DateTimeOffset.UtcNow - CreatedAt) > TimeSpan.FromMinutes(5))
            throw new DomainException("Edit window expired");
        Content = newContent;
        IsEdited = true;
        EditedAt = DateTimeOffset.UtcNow;
    }
    
    public void SoftDelete() { IsDeleted = true; Content = null; }
}

public enum MessageType
{
    Text = 1, Image = 2, Attachment = 3,
    OfferLink = 4, System = 5
}
```

### `MessageReadReceipt`, `MessageReport`, `UserBlock`

```csharp
public class MessageReadReceipt
{
    public Guid Id, MessageId, ConversationId, ReaderUserId;
    public DateTimeOffset ReadAt;
    // UNIQUE(message_id, reader_user_id)
}

public class MessageReport
{
    public Guid Id;
    public Guid? MessageId, ConversationId;
    public Guid ReporterUserId;
    public string Reason;                                        // "spam", "harassment", "scam"
    public string? Description;
    public ReportStatus Status;
    public Guid? ResolvedByUserId;
    public DateTimeOffset? ResolvedAt;
    public string? ResolutionNote;
    public DateTimeOffset CreatedAt;
}

// AR DEĞİL — junction
public class UserBlock
{
    public Guid Id;
    public Guid BlockerUserId, BlockedUserId;
    public string? Reason;
    public DateTimeOffset BlockedAt;
    // UNIQUE(blocker_user_id, blocked_user_id)
}
```

---

## 4. SignalR `/messaging/hub`

### Separate Hub (Karar 5/Messaging)

| Hub | Modül | URL |
|---|---|---|
| `/messaging/hub` | Messaging | Conversation realtime |
| `/notifications/hub` | Notifications | Cross-module notification stream |

**Sebep:** Modül izolasyonu, farklı lifecycle (bidirectional vs fire-and-forget), farklı scaling.

### `MessagingHub` İmza

```csharp
[Authorize]
public sealed class MessagingHub : Hub
{
    public override async Task OnConnectedAsync()
    {
        var userId = _currentUser.GetUserId()!.Value;
        await Groups.AddToGroupAsync(Context.ConnectionId, $"user:{userId}");
        
        var conversations = await _convRepo.GetActiveConversationIdsAsync(userId, default);
        foreach (var convId in conversations)
            await Groups.AddToGroupAsync(Context.ConnectionId, $"conv:{convId}");
    }
    
    // Client → Server
    public async Task SendTypingIndicator(Guid conversationId, bool isTyping)
    {
        var userId = _currentUser.GetUserId()!.Value;
        if (!await _convRepo.IsParticipantAsync(conversationId, userId, default)) return;
        
        await Clients.OthersInGroup($"conv:{conversationId}")
            .SendAsync("TypingIndicator", new { conversationId, userId, isTyping });
    }
    
    public async Task JoinConversation(Guid conversationId) { ... }
    public async Task LeaveConversation(Guid conversationId) { ... }
}
```

### Server → Client Events

- `MessageSent` { message: MessageDto }
- `MessageRead` { messageId, readerId, readAt }
- `MessageEdited` { messageId, newContent }
- `MessageDeleted` { messageId }
- `TypingIndicator` { conversationId, userId, isTyping }

### Typing Indicator — Pure WS

Karar: DB'ye yazılmıyor, event bus'a girmiyor, sadece SignalR group broadcast. Ephemeral.

---

## 5. Read Receipt Opt-Out (Server-Enforced)

```csharp
public sealed class MarkMessageReadHandler
{
    public async Task Consume(...)
    {
        var conv = await _db.Conversations.Include(c => c.Participants).FirstAsync(c => c.Id == cmd.ConversationId);
        var readerUserId = _currentUser.GetUserId()!.Value;
        
        var receipt = conv.MarkMessageRead(readerUserId, cmd.MessageId);
        _db.MessageReadReceipts.Add(receipt);
        
        // Opt-out check (Patch 1.1)
        var readerPrefs = await _identityRead.GetUserPreferencesAsync(readerUserId, ct);
        
        if (readerPrefs.ReadReceiptsEnabled)
        {
            // Public event yayınla
            await _publish.Publish(new MessageRead(...), ct);
        }
        // Aksi durumda sessiz okundu — karşı taraf bilmiyor
        
        await _uow.SaveChangesAsync(ct);
    }
}
```

---

## 6. Auto-Translate (On-Demand)

```
POST /me/messages/{id}/translate?targetLang=tr
   ↓ ISubscriptionReadService.GetTranslationQuotaRemainingAsync
   ↓ < 1 ise 403
   ↓ AI provider call (DeepL/OpenAI)
   ↓ ISubscriptionCommands.ConsumeTranslationQuotaAsync
   ↓ Cache Redis 24h (per messageId+targetLang)
   ↓ Response: { translatedContent, sourceLanguage, confidence }
```

**Thread-level toggle:** Frontend localStorage (Faz 2 server-persist `ConversationParticipant.AutoTranslateEnabled`).

**Quota:** Listings + Messaging shared `PlanFeatures.AiTranslationsPerMonth`. Granular Faz 2 (Backlog #133).

---

## 7. System Messages (12 Template)

### Tetikleyici Event Consumer'lar

```csharp
public sealed class OfferAcceptedSystemMessageHandler : IConsumer<OfferAccepted>
{
    public async Task Consume(ConsumeContext<OfferAccepted> ctx)
    {
        var conv = await _db.Conversations.FirstOrDefaultAsync(/* buyer/seller match */);
        if (conv is null)
        {
            conv = Conversation.Start(buyerUserId, sellerUserId, listingId, offerId, dealId);
            _db.Conversations.Add(conv);
        }
        
        var sysMessage = conv.PostMessage(
            senderUserId: SystemUserSentinel.Id,    // sabit Guid "Sistem"
            type: MessageType.System,
            systemKey: "offer.accepted",
            systemContextJson: JsonSerializer.Serialize(new { offerId, amount, currency }));
        
        _db.Messages.Add(sysMessage);
        await _uow.SaveChangesAsync(ctx.CancellationToken);
    }
}
```

### Render-Time Translation

```csharp
public sealed class SystemMessageRenderer
{
    public async Task<string> RenderAsync(string systemKey, string contextJson, LanguageCode locale, CancellationToken ct)
    {
        var template = await _templates.GetSystemMessageAsync(systemKey, locale, ct);
        return template.Render(contextJson);   // Mustache veya simple {{var}} substitution
    }
}
```

### Tetikleyici Listesi (12)

| Event | System Key | Template (TR) |
|---|---|---|
| `OfferSubmitted` | `offer.submitted` | "Teklif gönderildi: {amount} {currency}" |
| `OfferAccepted` | `offer.accepted` | "Teklif kabul edildi: {amount} {currency}" |
| `OfferRejected` | `offer.rejected` | "Teklif reddedildi" |
| `OfferCounterProposed` | `offer.countered` | "Karşı teklif: {amount} {currency}" |
| `DealCreated` (internal) | `deal.created` | "Anlaşma başlatıldı" |
| `DealPaymentConfirmed` | `deal.paid` | "Ödeme onaylandı" |
| `CarrierShipmentCreated` | `shipment.created` | "Kargo hazırlanıyor" |
| `CarrierShipmentPickedUp` | `shipment.picked_up` | "Kargo yola çıktı" |
| `CarrierShipmentDelivered` | `shipment.delivered` | "Kargo teslim edildi" |
| `DealCompleted` | `deal.completed` | "İşlem tamamlandı" |
| `DealCancelled` | `deal.cancelled` | "Anlaşma iptal edildi" |
| `DealDisputed` | `deal.disputed` | "Anlaşmazlık açıldı" |

Faz 1 seed: TR + EN zorunlu, AR + RU "varsa".

---

## 8. Block / Archive / Pin / Report

### Block Flow

```
POST /me/blocks { userId, reason? }
   ↓
1. UserBlock row insert (UNIQUE constraint idempotent)
2. Mevcut conversation'larda mesaj gönderim bloklanır (validator)
3. Mutual — blocked user da blocker'a mesaj gönderemez
4. Existing messages görünür kalır (history preserve)
```

**PostMessage validator:**

```csharp
RuleFor(x => x).MustAsync(async (cmd, ct) =>
{
    var senderId = _currentUser.GetUserId()!.Value;
    var conv = ...;
    var otherUserId = conv.OtherParticipantOf(senderId);
    
    var blocked = await _db.UserBlocks.AnyAsync(b => 
        (b.BlockerUserId == senderId && b.BlockedUserId == otherUserId)
        || (b.BlockerUserId == otherUserId && b.BlockedUserId == senderId),
        ct);
    return !blocked;
}).WithMessage("Conversation blocked");
```

### Archive/Pin (Participant-Level)

`participant.Archive()`, `participant.Pin()` — karşı taraf etkilenmiyor.

### Report

`POST /me/messages/{id}/report` veya `/conversations/{id}/report` → MessageReport row → `MessageReportFiled` Public event → admin moderation queue.

---

## 9. Attachments (Pre-Signed Upload)

Karar 5/Accounts Y1 pattern'i ile aynı:

```
POST /me/conversations/{id}/upload-url
   { fileName, contentType, sizeBytes }
   ↓ IFileStorage.GenerateSignedUploadUrlAsync(
       bucket: "message-attachments",
       key: "{convId}/{messageId}/{fileName}",
       contentType, ttl: 15min, maxSize: 20MB)
   ↓ Response: { uploadUrl, fileUrl, expiresAt }

Client → MinIO direct PUT

POST /me/conversations/{id}/messages
   { type: "attachment", attachmentUrl, attachmentMime, attachmentSizeBytes, attachmentFileName }
```

Max attachment: 20 MB per message.

---

## 10. Cross-Modül Erişim

### Tükettiği

| Modül | Method |
|---|---|
| Identity | `GetUserSummaryAsync`, `GetUserPreferencesAsync` (read receipts opt-out) |
| Listings | `GetSummaryAsync` (context rendering) |
| Marketplace | `GetOfferSummaryAsync`, `GetDealSummaryAsync` |
| Subscription | `GetTranslationQuotaRemainingAsync` + `ConsumeTranslationQuotaAsync` |
| FileStorage | Pre-signed upload URL |

### Sunduğu

```csharp
public interface IMessagingReadService
{
    Task<int> GetUnreadCountAsync(Guid userId, CancellationToken ct);
    Task<int> GetUnreadConversationCountAsync(Guid userId, CancellationToken ct);
    Task<ConversationSummary?> GetConversationSummaryAsync(Guid conversationId, CancellationToken ct);
    Task<bool> IsUserBlockedAsync(Guid blockerUserId, Guid blockedUserId, CancellationToken ct);
}

public interface IAdminMessagingCommands
{
    Task<Result> ResolveReportAsync(Guid reportId, string resolutionNote, Guid adminUserId, CancellationToken ct);
    Task<Result> DeleteMessageAsync(Guid messageId, string reason, Guid adminUserId, CancellationToken ct);
    Task<Result> CloseConversationAsync(Guid conversationId, string reason, Guid adminUserId, CancellationToken ct);
}
```

---

## 11. Public Event'ler (4)

| Event | Consumer |
|---|---|
| `ConversationStarted` | Notifications (recipient'e ilk-mesaj bildir) |
| `MessageSent` | Notifications (push, in-app counter, email digest) — yüksek freq |
| `MessageRead` | Notifications (in-app counter güncelle) — opt-out filter |
| `MessageReportFiled` | Notifications (admin moderation queue) |

**Internal:** ConversationArchived, MessageEdited, MessageDeleted.
**Pure WS (no event bus):** TypingIndicator.

---

## 12. API Endpoint Inventory (25)

### Authenticated — Conversations (10)

`GET/POST /me/conversations` + `/{id}` + `/archive` + `/unarchive` + `/pin` + `/unpin` + `/read-all` + `/report` + `/upload-url`.

### Authenticated — Messages (7)

`GET /me/conversations/{id}/messages?cursor=&direction=` + `POST /messages` + `/read` + `/translate?targetLang=` + `/report` + `PATCH/DELETE /me/messages/{id}`.

### Authenticated — Blocks (3)

`GET /me/blocks` + `POST /me/blocks` + `DELETE /me/blocks/{userId}`.

### Authenticated — Unread (1)

`GET /me/messaging/unread-summary` → `{ total, conversationCount }`.

### Admin (3)

`GET /admin/messaging/reports?status=&cursor=` + `/{id}/resolve` + `/messages/{id}/delete`.

### WebSocket Hub (1)

`WS /messaging/hub` — typing, message events.

---

## 13. Discovered Backlog

| # | Konu | Hedef |
|---|---|---|
| 133 | Granular message translation quota Faz 2 | Karar 5 / Subscription |
| 134 | System message template seed (12 × 4 locale = 48 row) | Karar 5 / Messaging seed |
| 135 | Group chat Faz 2 (3+ participant) | Karar 7 / Faz 2 |
| 136 | Message edit history Faz 2 | Karar 7 / Audit |
| 137 | MessagingHub Redis backplane Faz 2 (multi-instance) | Karar 7 / Scale-out |
| 138 | SystemUserSentinel sabit Guid (`00000000-0000-0000-0000-000000000001`) | Karar 5 / Messaging Faz 1 detay |
| 139 | Attachment thumbnail generation Faz 2 (256/512 thumb async worker) | Karar 7 |
| 140 | Message search (PostgreSQL FTS) Faz 2 | Karar 7 / Search |
| 141 | UserPreferences ReadReceiptsEnabled + TypingIndicatorEnabled (kapandı patch) | ✓ |

---

## 14. Özet Tablo

| Konu | Karar |
|---|---|
| AR sayısı | 1 (Conversation); child entities Message, ConversationParticipant, MessageReadReceipt, MessageReport |
| Junction (AR değil) | UserBlock |
| Public events | 4 |
| Hub | Separate `/messaging/hub` (Karar 5/Messaging Q1) |
| Typing | Pure WS broadcast, no persist, no event bus |
| Read receipts | Opt-out server-enforced (Identity prefs); event publish gated |
| Auto-translate | On-demand endpoint, client-side toggle (Faz 2 server-persist) |
| System messages | 12 template; event consumer otomatik post |
| Block | Mutual (her iki yönde); UserBlock junction Messaging içinde |
| Attachments | Pre-signed URL, max 20MB |
| Endpoint | 25 (24 HTTP + 1 WS Hub) |
