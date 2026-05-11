# Karar 5 / Marketplace Modülü

**Status:** FINAL (Agreement → Deal rename uygulandı, Backlog #65 kapandı)
**Wave:** 6 (parallel with Messaging + Notifications)

## İlişkili Kararlar

- **Üst:** [Karar 2](../02-modules-list.md), [Karar 3a — AR](../03-domain-patterns.md)
- **Patch:** [Patch 5 — Görev 3 Communication Matrix](../05-patch.md) — Deal rename (Agreement → Deal); AR Co-creation Pattern (Offer.Accept → Deal aynı TX)
- **Frontend:** `frontend-api-inventory.md` Marketplace (14 endpoint — offer, counter, deal, dispute, favorite, payment)

---

## 1. Modülün Rolü ve Sınırları

### Sahip

| Konsept | Sahiplik |
|---|---|
| Offer AR (buyer/seller counter offer chain) | Marketplace |
| Deal AR (accepted Offer → Deal, 8-state FSM) | Marketplace |
| Favorite AR (kullanıcı favori listing) | Marketplace |
| Dispute AR (Deal'a açılan anlaşmazlık + Evidence append-only) | Marketplace |
| Escrow state tracking (Faz 1 in-DB; Faz 2 gerçek para) | Marketplace |
| Counter offer chain (parent_offer_id graph) | Marketplace |
| Payment method routing (Subscription PM cross-module read) | Marketplace (UI proxy) |

### Sahip Olmayan

| Konsept | Sahibi |
|---|---|
| Listing CRUD | Listings |
| Seller business profile | Accounts |
| Shipment lifecycle | Carrier |
| Commission rate calculation | Subscription |
| Payment method physical storage | Subscription (universal Stripe customer) |
| Review yazma | Accounts (Reviews AR) |
| Conversation/Messages | Messaging |

---

## 2. Aggregate Roots

### `Offer` AR (Counter Chain + Direction Flip)

```csharp
public class Offer
{
    public Guid Id { get; private set; }
    public Guid ListingId;                                       // Single listing Faz 1 (Faz 2 multi-listing Backlog #127)
    public Money ListingPriceSnapshot;                           // create anında listing fiyatı
    
    public Guid BuyerUserId, SellerUserId, SellerId;
    
    public Money Amount;
    public int Quantity;
    public string? Message;
    
    public Guid? ParentOfferId;                                  // counter offer chain
    public int CounterDepth;                                     // 0 = original, max 5
    public OfferStatus Status;
    public OfferDirection Direction;                             // BuyerToSeller | SellerToBuyer (counter)
    
    public DateTimeOffset CreatedAt;
    public DateTimeOffset ExpiresAt;                             // default 7 day
    public DateTimeOffset? AcceptedAt, RejectedAt, CountedAt, WithdrawnAt;
    public Guid? AcceptedActorUserId;
    public Guid? ResultingDealId;                                // populated on Accept
    
    public DateTimeOffset UpdatedAt;
    
    public static Offer CreateInitial(
        Guid listingId, Money listingPriceSnapshot,
        Guid buyerUserId, Guid sellerUserId, Guid sellerId,
        Money amount, int quantity, string? message, TimeSpan ttl) 
    {
        return new Offer
        {
            Id = Guid.CreateVersion7(),
            CounterDepth = 0,
            Status = OfferStatus.Pending,
            Direction = OfferDirection.BuyerToSeller,
            ExpiresAt = DateTimeOffset.UtcNow.Add(ttl),
        };
        // Public event: OfferSubmitted
    }
    
    public Offer CreateCounter(Money newAmount, int? newQuantity, string? message, TimeSpan ttl)
    {
        if (Status != OfferStatus.Pending) throw new DomainException();
        if (CounterDepth >= 5) throw new DomainException("Max 5");
        
        Status = OfferStatus.Countered;
        CountedAt = DateTimeOffset.UtcNow;
        
        var counter = new Offer
        {
            Id = Guid.CreateVersion7(),
            ListingId = ListingId,
            ListingPriceSnapshot = ListingPriceSnapshot,
            BuyerUserId = BuyerUserId, SellerUserId = SellerUserId, SellerId = SellerId,
            Amount = newAmount, Quantity = newQuantity ?? Quantity, Message = message,
            ParentOfferId = Id,
            CounterDepth = CounterDepth + 1,
            Status = OfferStatus.Pending,
            Direction = Direction == OfferDirection.BuyerToSeller 
                ? OfferDirection.SellerToBuyer 
                : OfferDirection.BuyerToSeller,
            ExpiresAt = DateTimeOffset.UtcNow.Add(ttl),
        };
        return counter;
        // Public event: OfferCounterProposed
    }
    
    public Deal Accept(Guid actorUserId, CarrierAttachment? carrier)
    {
        if (Status != OfferStatus.Pending) throw new DomainException();
        
        // Direction'a göre actor doğru taraf mı?
        var expectedActor = Direction == OfferDirection.BuyerToSeller ? SellerUserId : BuyerUserId;
        if (actorUserId != expectedActor) throw new DomainException("Wrong actor");
        
        Status = OfferStatus.Accepted;
        AcceptedAt = DateTimeOffset.UtcNow;
        AcceptedActorUserId = actorUserId;
        
        // AR Co-creation Pattern (Backlog #8 — aynı modül + aynı TX)
        var deal = Deal.CreateFromOffer(this, carrier);
        ResultingDealId = deal.Id;
        return deal;
        // Public events: OfferAccepted + DealCreated (Internal Faz 1)
    }
    
    public void Reject(Guid actorUserId, string? reason) { /* OfferRejected */ }
    public void Withdraw(Guid actorUserId) { /* OfferWithdrawn */ }
    public void Expire() { /* OfferExpired (cron job) */ }
}

public enum OfferStatus { Pending = 1, Accepted = 2, Rejected = 3, Countered = 4, Withdrawn = 5, Expired = 6 }
public enum OfferDirection { BuyerToSeller = 1, SellerToBuyer = 2 }

public sealed record CarrierAttachment(Guid CarrierId, Money CarrierFee, DateTimeOffset RequestedPickupAt);
```

### `Deal` AR (eski Agreement — rename uygulandı)

```csharp
public class Deal
{
    public Guid Id;
    public Guid OfferId;                                         // IMMUTABLE
    public Guid ListingId;
    
    public Guid BuyerUserId, SellerUserId, SellerId;
    
    public Money Amount;
    public int Quantity;
    
    // Carrier (opsiyonel — seller kendi taşır seçeneği var)
    public Guid? CarrierId;
    public Money? CarrierFee;
    public DateTimeOffset? RequestedPickupAt;
    public Guid? ShipmentId;                                     // Carrier shipment AR ID-ref
    
    // Escrow (Faz 1 in-DB tracking; Faz 2 gerçek para)
    public Money EscrowAmount;
    public EscrowStatus EscrowStatus;
    public DateTimeOffset? EscrowHeldAt, EscrowReleasedAt, EscrowRefundedAt;
    
    // Payment
    public string? StripePaymentIntentId, StripeChargeId;
    public Guid? InvoiceId;                                      // Subscription.Invoice (commission)
    
    public DealStatus Status;
    public DateTimeOffset CreatedAt;
    public DateTimeOffset? PaidAt, PreparedAt, PickedUpAt, DeliveredAt, CompletedAt, CancelledAt, DisputedAt;
    public CancellationReason? CancelReason;
    public string? CancelNote;
    public DateTimeOffset UpdatedAt;
    
    // Commission snapshot
    public decimal CommissionRatePercent;
    public Money? CommissionAmount;
    
    public static Deal CreateFromOffer(Offer offer, CarrierAttachment? carrier)
    {
        return new Deal
        {
            Id = Guid.CreateVersion7(),
            OfferId = offer.Id,
            ListingId = offer.ListingId,
            BuyerUserId = offer.BuyerUserId, SellerUserId = offer.SellerUserId, SellerId = offer.SellerId,
            Amount = offer.Amount, Quantity = offer.Quantity,
            CarrierId = carrier?.CarrierId,
            CarrierFee = carrier?.CarrierFee,
            EscrowAmount = offer.Amount + (carrier?.CarrierFee ?? Money.Zero(offer.Amount.Currency)),
            EscrowStatus = EscrowStatus.Pending,
            Status = DealStatus.PendingPayment,
            CommissionRatePercent = 0m,                          // populate later
        };
        // Internal event: DealCreated (Görev 3 Q1 — Offer payload'da DealId yeter)
    }
    
    public void RecordPayment(string paymentIntentId, string chargeId, DateTimeOffset paidAt) 
    { 
        Status = DealStatus.Paid;
        EscrowStatus = EscrowStatus.Held;
        EscrowHeldAt = paidAt;
        // Public event: DealPaymentConfirmed
    }
    
    public void MoveToInPreparation() { Status = DealStatus.InPreparation; }
    public void AttachShipment(Guid shipmentId) { ShipmentId = shipmentId; }
    public void MarkPickedUp(DateTimeOffset pickedUpAt) { /* DealShipmentStarted */ }
    public void MarkDelivered(DateTimeOffset deliveredAt) { /* DealDelivered */ }
    
    public void Complete(decimal commissionRatePercent)
    {
        Status = DealStatus.Completed;
        CompletedAt = DateTimeOffset.UtcNow;
        EscrowStatus = EscrowStatus.Released;
        EscrowReleasedAt = DateTimeOffset.UtcNow;
        CommissionRatePercent = commissionRatePercent;
        CommissionAmount = Amount * (commissionRatePercent / 100m);
        // Public event: DealCompleted — 6-consumer cascade
    }
    
    public void Cancel(CancellationReason reason, string? note, Guid actorUserId)
    {
        if (Status is DealStatus.InTransit or DealStatus.Delivered or DealStatus.Completed)
            throw new DomainException("Use Dispute for InTransit+");
        Status = DealStatus.Cancelled;
        // Public event: DealCancelled
    }
    
    public void MarkDisputed() { /* DealDisputed; status değişmez, Dispute AR paralel */ }
    public void ResolveDispute(DisputeResolutionKind resolution, Money? refundAmount) { /* DealResolved */ }
}

public enum DealStatus
{
    PendingPayment = 1, Paid = 2, InPreparation = 3,
    InTransit = 4, Delivered = 5, Completed = 6,
    Disputed = 7,         // paralel flag
    Cancelled = 8
}

public enum EscrowStatus { Pending = 1, Held = 2, Released = 3, Refunded = 4 }
public enum CancellationReason { UserRequested = 1, PaymentFailed = 2, SellerCancelled = 3, AdminInitiated = 4 }
```

### `Dispute` AR (Evidence Append-Only)

```csharp
public class Dispute
{
    public Guid Id;
    public Guid DealId;
    public Guid RaisedByUserId;
    public DisputePartyRole RaisedByRole;                        // Buyer | Seller
    
    public string Reason;                                        // "AnimalConditionNotAsDescribed", "NonDelivery", ...
    public string Description;
    public DisputeStatus Status;                                 // Raised → UnderReview → Resolved
    
    public DateTimeOffset RaisedAt;
    public DateTimeOffset? UnderReviewAt, ResolvedAt;
    public Guid? AssignedAdminUserId, ResolvedByUserId;
    public DisputeResolution? Resolution;
    public DateTimeOffset UpdatedAt;
    
    private readonly List<DisputeEvidence> _evidence = new();
    
    public static Dispute Raise(Guid dealId, Guid raisedByUserId, DisputePartyRole role, string reason, string description) { ... }
    
    public DisputeEvidence AddEvidence(Guid submittedByUserId, EvidenceKind kind, string? text, string? attachmentUrl)
    {
        if (Status == DisputeStatus.Resolved)
            throw new DomainException("Cannot add to resolved");
        var e = new DisputeEvidence(Id, submittedByUserId, kind, text, attachmentUrl);
        _evidence.Add(e);
        return e;
        // IMMUTABLE — silinmez, edit edilmez
    }
    
    public void StartReview(Guid adminUserId) { /* UnderReview */ }
    public void Resolve(Guid adminUserId, DisputeResolution resolution) { /* Resolved */ }
}

public enum DisputeStatus { Raised = 1, UnderReview = 2, Resolved = 3 }
public enum DisputePartyRole { Buyer = 1, Seller = 2 }
public enum EvidenceKind { Photo = 1, Document = 2, Text = 3, ChatLog = 4, ShipmentRecord = 5 }
public enum DisputeResolutionKind { RefundFull = 1, RefundPartial = 2, CompleteAsIs = 3, Custom = 99 }

public sealed record DisputeResolution(DisputeResolutionKind Kind, Money? RefundAmount, string DecisionNote);

public class DisputeEvidence
{
    public Guid Id, DisputeId, SubmittedByUserId;
    public EvidenceKind Kind;
    public string? Text, AttachmentUrl;                          // IFileStorage signed URL
    public DateTimeOffset SubmittedAt;
    // IMMUTABLE — no edit/delete methods
}
```

### `Favorite` AR

```csharp
public class Favorite
{
    public Guid Id;
    public Guid UserId, ListingId;
    public string? Tag;                                          // opsiyonel "compare-later"
    public DateTimeOffset AddedAt;
    
    public static Favorite Add(Guid userId, Guid listingId, string? tag) { ... }
}
```

DB: `UNIQUE(user_id, listing_id)`. Idempotent toggle.

---

## 3. AR Co-Creation Pattern (Backlog #8)

Offer.Accept() → Deal yaratma **aynı modül + aynı transaction** (Karar 3d istisnası):

```csharp
public sealed class AcceptOfferHandler : IConsumer<AcceptOfferCommand>
{
    public async Task Consume(ConsumeContext<AcceptOfferCommand> ctx)
    {
        var userId = _currentUser.GetUserId()!.Value;
        var offer = await _db.Offers.FirstAsync(o => o.Id == cmd.OfferId, ct);
        
        // Cross-modül validators (sync)
        var seller = await _accounts.GetSellerSummaryAsync(offer.SellerId, ct);
        if (seller is null || seller.Status != OnboardingStatus.Verified)
            throw new ConflictException("SellerNotVerified");
        
        var listing = await _listings.GetSummaryAsync(offer.ListingId, ct);
        if (listing is null || listing.Status != ListingStatus.Active)
            throw new ConflictException("ListingNotActive");
        
        // Domain — Offer.Accept() Deal yaratır (AR co-creation aynı TX)
        var deal = offer.Accept(userId, carrierAtt);
        _db.Deals.Add(deal);
        
        // Cross-modül sync: Listing reserve
        var reserveResult = await _listingsCommands.ReserveAsync(offer.ListingId, deal.Id, ct);
        if (reserveResult.IsFailure)
            throw new ConflictException("ListingReservationFailed");
        
        // Outbox events
        await _publish.Publish(new OfferAccepted(...), ct);
        
        await _uow.SaveChangesAsync(ct);   // Offer + Deal aynı TX
        
        await ctx.RespondAsync(new AcceptOfferResponse(deal.Id));
    }
}
```

---

## 4. Payment Flow (Stripe Faz 1 Mock)

```
1. Deal PendingPayment → Buyer "Ödeme Yap" tıklar
2. POST /me/deals/{id}/checkout
   → ISubscriptionReadService.GetPaymentMethodsAsync(buyerUserId)
   → Frontend PM seç
   → ISubscriptionCommands.ChargeAsync(buyerUserId, paymentMethodId, deal.EscrowAmount, ...)
     - Mock: success → mockChargeId
     - Real (Faz 2): Stripe PaymentIntent create + confirm
3. deal.RecordPayment(paymentIntentId, chargeId, paidAt)
   → DealStatus → Paid, EscrowStatus → Held
4. Public event: DealPaymentConfirmed
   → Carrier shipment trigger (sync ICarrierShipmentCommands.CreateAsync varsa)
   → Notifications: seller'a "Ödeme alındı"
```

### Payment Method Endpoints

Marketplace'in **kendi PM endpoint'i yok** — frontend doğrudan `/subscription/me/payment-methods` çağırır (Subscription Patch 2.4 — universal Stripe customer).

---

## 5. Carrier Shipment Trigger (Görev 3 Cascade Chain 4)

```csharp
public sealed class DealPaymentConfirmedHandler : IConsumer<DealPaymentConfirmed>
{
    private readonly ICarrierShipmentCommands _carrierCommands;
    
    public async Task Consume(ConsumeContext<DealPaymentConfirmed> ctx)
    {
        var deal = await _db.Deals.FirstAsync(d => d.Id == ctx.Message.DealId);
        
        if (!deal.CarrierId.HasValue)
        {
            deal.MoveToInPreparation();
            await _uow.SaveChangesAsync(ctx.CancellationToken);
            return;
        }
        
        // Sync command Carrier'a
        var result = await _carrierCommands.CreateAsync(
            dealId: deal.Id,
            carrierId: deal.CarrierId.Value,
            details: BuildShipmentDetails(deal),
            agreedPrice: deal.CarrierFee!.Value,
            ctx.CancellationToken);
        
        if (result.IsFailure)
        {
            // Compensating action / admin escalation
            throw new ConflictException($"Shipment create failed: {result.Error}");
        }
        
        deal.AttachShipment(result.Value);
        deal.MoveToInPreparation();
        await _uow.SaveChangesAsync(ctx.CancellationToken);
    }
}
```

---

## 6. Commission Cascade (DealCompleted)

```csharp
public sealed class DealCompletedCommissionHandler : IConsumer<DealCompleted>
{
    public async Task Consume(ConsumeContext<DealCompleted> ctx)
    {
        var deal = await _db.Deals.FirstAsync(d => d.Id == ctx.Message.DealId);
        
        // Resolve commission rate cross-modül
        var listing = await _listings.GetSummaryAsync(deal.ListingId, ctx.CancellationToken);
        var commissionRate = await _subRead.ResolveCommissionRateAsync(
            sellerUserId: deal.SellerUserId,
            categoryId: listing!.CategoryId, 
            ctx.CancellationToken);
        
        deal.Complete(commissionRate);
        
        var commissionAmount = deal.Amount * (commissionRate / 100m);
        
        // Sync charge commission via Subscription
        var chargeResult = await _subCommands.ChargeCommissionAsync(
            sellerUserId: deal.SellerUserId,
            dealId: deal.Id,
            amount: commissionAmount,
            ctx.CancellationToken);
        // Subscription publishes CommissionCharged → Notifications seller, Admin revenue
        
        await _uow.SaveChangesAsync(ctx.CancellationToken);
    }
}
```

---

## 7. Counter Offer Chain Örneği

```
Listing fiyat: 100K TRY
Offer #1: Buyer → Seller, 80K TRY (BuyerToSeller, depth=0)
   ↓ Seller "Counter"
Offer #2: Seller → Buyer, 95K TRY (SellerToBuyer, depth=1, parent=#1)
   ↓ Offer #1 status: Countered
   ↓ Buyer "Counter"
Offer #3: Buyer → Seller, 88K TRY (BuyerToSeller, depth=2, parent=#2)
   ↓ Seller "Accept"
Deal yaratıldı, Amount=88K
   ↓ Offer #3 status: Accepted
   ↓ #2, #1 status: Countered (zaten)
```

Max counter depth = 5 (UX guard).

---

## 8. Cross-Modül Erişim

### Tükettiği

| Pattern | Modül | Method |
|---|---|---|
| Validator | Listings | `GetSummaryAsync` |
| Validator | Accounts | `IsActiveSellerAsync` |
| Reserve/Unreserve/MarkSold | Listings | `IListingsCommands.*` (sync) |
| Shipment create | Carrier | `ICarrierShipmentCommands.CreateAsync` (sync) |
| Shipment cancel | Carrier | `CancelByDealAsync` |
| Charge | Subscription | `ISubscriptionCommands.ChargeAsync` |
| Refund | Subscription | `RefundAsync` |
| Commission charge | Subscription | `ChargeCommissionAsync` |
| Payment methods | Subscription | `GetPaymentMethodsAsync` |
| Commission rate | Subscription | `ResolveCommissionRateAsync` |
| User details | Identity | `GetUserSummaryAsync` |

### Sunduğu

```csharp
public interface IMarketplaceReadService
{
    Task<DealSummary?> GetDealSummaryAsync(Guid dealId, CancellationToken ct);
    Task<IReadOnlyList<DealSummary>> ListUserDealsAsync(Guid userId, DealRole role, CancellationToken ct);
    Task<bool> IsDealCompletedAsync(Guid dealId, CancellationToken ct);
    Task<(Guid buyerUserId, Guid sellerUserId)?> GetDealParticipantsAsync(Guid dealId, CancellationToken ct);
    
    Task<BuyerStatsSlice> GetBuyerStatsAsync(Guid buyerUserId, CancellationToken ct);   // Accounts D4
    Task<int> GetActiveOfferCountAsync(Guid userId, OfferRole role, CancellationToken ct);
    Task<int> GetOpenDisputeCountAsync(Guid userId, CancellationToken ct);
}

public interface IAdminMarketplaceCommands
{
    Task<Result> CancelDealAsync(Guid dealId, string reason, Guid actorId, CancellationToken ct);
    Task<Result> StartDisputeReviewAsync(Guid disputeId, Guid adminUserId, CancellationToken ct);
    Task<Result> ResolveDisputeAsync(Guid disputeId, DisputeResolution resolution, Guid adminUserId, CancellationToken ct);
    Task<Result> RefundDealAsync(Guid dealId, Money? amount, string reason, Guid actorId, CancellationToken ct);
}
```

---

## 9. Public Event'ler (13)

### Offer (6)

| Event | Consumer |
|---|---|
| `OfferSubmitted` | Messaging (conversation), Notifications (seller) |
| `OfferAccepted` | Listings (Reserve), Messaging (system message), Notifications (buyer) |
| `OfferRejected` | Notifications |
| `OfferWithdrawn` | Notifications |
| `OfferCounterProposed` | Messaging, Notifications |
| `OfferExpired` | Notifications |

### Deal (7 — Agreement rename uygulandı)

| Event | Consumer |
|---|---|
| `DealPaymentConfirmed` | Carrier (ShipmentCreate sync), Notifications |
| `DealShipmentStarted` | Notifications buyer |
| `DealDelivered` | Notifications buyer (confirm prompt) |
| `DealCompleted` | Listings (MarkSold cascade), Subscription (ChargeCommission), Carrier, Messaging, Notifications, Admin (6 consumer) |
| `DealDisputed` | Notifications (admin queue + opposite party) |
| `DealResolved` | Notifications |
| `DealCancelled` | Listings (Unreserve), Notifications |

**Internal:** `DealCreated` (Görev 3 Q1 — Offer payload yeter).

---

## 10. API Endpoint Inventory (28)

### Authenticated — Offers (7)

`POST /listings/{id}/offers` + `GET /me/offers?role=&status=` + `/{id}` + `/accept` + `/reject` + `/counter` + `/withdraw`.

### Authenticated — Deals (6)

`GET /me/deals?role=&status=` + `/{id}` + `POST /checkout` + `/confirm-receipt` + `/cancel` + `/dispute`.

### Authenticated — Disputes (3)

`GET /me/disputes?cursor=` + `/{id}` + `POST /evidence`.

### Authenticated — Favorites (3)

`GET /me/favorites?cursor=` + `POST/DELETE /favorites/{listingId}`.

### Webhook (1)

`POST /marketplace/webhooks/payment` — Stripe signature.

### Admin (8)

`GET/POST /admin/marketplace/deals?status=&cursor=` + `/{id}` + `/cancel` + `/refund` + `GET /disputes?status=&cursor=` + `/{id}` + `/start-review` + `/resolve`.

---

## 11. Discovered Backlog

| # | Konu | Hedef |
|---|---|---|
| 8 | AR Co-creation Pattern (kapandı) | ✓ |
| 65 | Deal naming alignment (kapandı) | ✓ |
| 123 | `ISubscriptionCommands.ChargeCommissionAsync` (kapandı Patch 2) | ✓ |
| 124 | Partial refund Faz 2 | Karar 7 |
| 125 | Counter offer chain max depth UX | Karar 6 / API contract |
| 126 | Offer expiry cron job | Karar 5 / Marketplace cron |
| 127 | Multi-listing offer Faz 2 | Karar 7 / Faz 2 |
| 128 | Dispute auto-resolution timeout (admin response 14gün) | Karar 7 / Faz 2 |
| 129 | Buyer "Confirm Receipt" auto-deadline (Delivered + 7gün) | Karar 5 / Marketplace cron |
| 130 | Stripe Connect Faz 2 | Karar 7 / Payment infra |
| 131 | Listing snapshot at Offer time (Faz 2 full snapshot) | Karar 5 / Marketplace ek |
| 132 | `/marketplace/me/payment-methods` alias vs Subscription direct (kapandı — Subscription direct) | ✓ |

---

## 12. Özet Tablo

| Konu | Karar |
|---|---|
| AR sayısı | 4 (Offer, Deal, Favorite, Dispute) |
| Deal rename | ✓ Agreement* → Deal* uygulandı |
| Public events | 13 (6 Offer + 7 Deal) |
| Offer FSM | 6 state |
| Deal FSM | 8 state (Disputed/Cancelled paralel) |
| Counter offer | Chain max depth 5, direction flip |
| Multi-listing offer | Faz 1 single-only |
| AR co-creation | Offer.Accept() → Deal aynı modül + aynı TX (Backlog #8 onaylı) |
| Carrier trigger | Sync `ICarrierShipmentCommands.CreateAsync` DealPaymentConfirmed'ta |
| Commission cascade | DealCompleted → `ISubscriptionCommands.ChargeCommissionAsync` |
| Escrow | Faz 1 state-tracking; Faz 2 gerçek para Stripe |
| Payment methods | Subscription module owns; Marketplace cross-modül read |
| Dispute Evidence | Append-only (silinmez, edit yasak) |
| Endpoint | 28 (19 auth + 1 webhook + 8 admin) |
