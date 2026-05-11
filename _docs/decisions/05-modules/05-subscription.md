# Karar 5 / Subscription Modülü

**Status:** FINAL
**Wave:** 5 (parallel with Carrier)

## İlişkili Kararlar

- **Üst:** [Karar 2 revize](../02-modules-list.md) — Subscription Accounts'tan ayrıldı
- **Patch:** [Patch 2 — Subscription](../05-patch.md) — `ISubscriptionCommands.ChargeAsync/RefundAsync/ChargeCommissionAsync`, quota consume API'leri, `PaymentMethod` entity (universal), `PlanFeatures.ListingRefreshesPerMonth + AiTranslationsPerMonth`
- **Frontend:** `frontend-api-inventory.md` Subscription (16 endpoint — pricing, subscribe, invoices, boost)
- **Görev 1/F kararı:** `subscriber_user_id` future-positive naming (UserId reference), Faz 1 seller-only guard application-layer
- **Çekince 1:** BoostKind 2 enum (FeaturedListing/HomepageBanner) + 3 paket (top-row/showcase/urgent-label); refresh/translation/verified_breeding doğru modüllerde
- **Çekince 2:** BoostCampaign tek listing (`AppliedListingId` Guid, çoklu yerine N campaign tek invoice altında)
- **Çekince 3:** BoostCampaign analytics computed (cross-module IListingsReadService.GetBoostAnalyticsAsync)

---

## 1. Modülün Rolü ve Sınırları

### Sahip

| Konsept | Sahiplik |
|---|---|
| Plan tanımları (Free/Standard/Pro/Enterprise + custom) | Subscription |
| Commission rule per plan (per-category Faz 2) | Subscription (Plan AR child) |
| Active subscription per user | Subscription |
| Trial period management (Faz 1 VAR) | Subscription |
| Invoice + billing | Subscription |
| **Payment method (Stripe customer ID, universal — buyer + seller)** | Subscription (Patch 2) |
| BoostPackage admin catalog | Subscription |
| BoostCampaign (purchased boost application) | Subscription |
| Prorate calculation (upgrade immediate, downgrade deferred) | Subscription |
| Grace period (InvoiceFailed → 7 day) | Subscription |
| MRR analytics (read model) | Subscription (Admin reads) |

### Sahip Olmayan

| Konsept | Sahibi |
|---|---|
| User identity, role assignment | Identity |
| Seller persona detayları | Accounts |
| Listing'in boost flag uygulama side-effect | Listings (BoostActivated event consumer) |
| Payment provider physical entegrasyon (Stripe API) | Subscription orchestration, Stripe SDK |

---

## 2. Aggregate Roots

### `Plan` AR

```csharp
public class Plan
{
    public Guid Id { get; private set; }
    public string Code { get; private set; }                   // "free", "standard", "pro", "enterprise"
    public Translations Name { get; private set; }
    public Translations? Description { get; private set; }
    public PlanTier Tier { get; private set; }
    public bool IsActive, IsPublic;
    public int DisplayOrder;
    
    public PlanFeatures Features { get; private set; }         // VO
    public int? TrialDays { get; private set; }                // null = trial yok
    
    private readonly List<PlanPricing> _pricings = new();      // currency variants
    private readonly List<CommissionRule> _commissionRules = new();
    
    public static Plan Create(string code, Translations name, PlanTier tier, PlanFeatures features, int? trialDays = null) { ... }
    
    public PlanPricing SetPricing(CurrencyCode currency, Money monthly, Money yearly, decimal? yearlyDiscountPercent) { /* replace if exists */ }
    
    public CommissionRule SetCommissionRule(int? categoryId, decimal ratePercent, Money? minAmount, Money? maxAmount)
    {
        // categoryId null = plan default; non-null = category-specific override
    }
    
    public decimal ResolveCommissionRate(int categoryId)
    {
        var specific = _commissionRules.FirstOrDefault(r => r.CategoryId == categoryId);
        if (specific is not null) return specific.RatePercent;
        var defaultRule = _commissionRules.FirstOrDefault(r => r.CategoryId is null);
        return defaultRule?.RatePercent ?? 0m;
    }
    
    public void Deactivate() { /* aktif subscription varsa engelle cross-modül validator */ }
}

public enum PlanTier { Free = 1, Standard = 2, Premium = 3, Enterprise = 4 }
```

### `Subscription` AR

```csharp
public class Subscription
{
    public Guid Id { get; private set; }
    public Guid SubscriberUserId { get; private set; }         // Görev 1/F future-positive naming
    public Guid PlanId { get; private set; }
    public string PlanCodeSnapshot { get; private set; }
    
    public BillingCycle Cycle;
    public CurrencyCode Currency;
    public Money RenewalPrice;                                  // snapshot at activation
    
    public DateTimeOffset PeriodStart, PeriodEnd;
    public bool AutoRenew;
    
    public SubscriptionStatus Status;                           // Trial/Active/PastDue/Cancelled/Expired
    public DateTimeOffset? TrialEndsAt, GracePeriodEndsAt, CancelledAt;
    public CancellationReason? CancelReason;
    public string? CancelNote;
    
    public string? StripeCustomerId, StripeSubscriptionId;
    public string? PaymentMethodLast4;
    public PaymentMethodKind? PaymentMethodKind;
    
    public DateTimeOffset CreatedAt, UpdatedAt;
    
    public static Subscription Activate(
        Guid subscriberUserId, Guid planId, string planCode,
        BillingCycle cycle, CurrencyCode currency, Money renewalPrice,
        int? trialDays, string? stripeCustomerId, string? stripeSubId)
    {
        var now = DateTimeOffset.UtcNow;
        var trialEnd = trialDays.HasValue ? now.AddDays(trialDays.Value) : (DateTimeOffset?)null;
        var periodEnd = cycle == BillingCycle.Monthly ? now.AddMonths(1) : now.AddYears(1);
        
        return new Subscription
        {
            Id = Guid.CreateVersion7(),
            SubscriberUserId = subscriberUserId, PlanId = planId, PlanCodeSnapshot = planCode,
            Cycle = cycle, Currency = currency, RenewalPrice = renewalPrice,
            PeriodStart = now,
            PeriodEnd = trialEnd ?? periodEnd,
            AutoRenew = true,
            Status = trialEnd.HasValue ? SubscriptionStatus.Trial : SubscriptionStatus.Active,
            TrialEndsAt = trialEnd,
            StripeCustomerId = stripeCustomerId, StripeSubscriptionId = stripeSubId,
        };
        // Public event: SubscriptionActivated
    }
    
    public void Renew(DateTimeOffset newPeriodEnd, Money? newPrice) { /* SubscriptionRenewed */ }
    public void Cancel(CancellationReason reason, string? note, Guid actorUserId) { /* SubscriptionCancelled */ }
    public void Expire() { /* SubscriptionExpired */ }
    
    public void Upgrade(Guid newPlanId, string newPlanCode, Money newPrice, Money prorateCharge) 
    {
        // PeriodEnd değişmez; prorate charge immediate
        // Public event: SubscriptionUpgraded
    }
    
    public void Downgrade(Guid newPlanId, string newPlanCode, Money newPrice)
    {
        // Period sonrası geçerli (deferred)
        // Public event: SubscriptionDowngraded
    }
    
    public void EnterGracePeriod(int days = 7)
    {
        Status = SubscriptionStatus.PastDue;
        GracePeriodEndsAt = DateTimeOffset.UtcNow.AddDays(days);
    }
    
    public void ExitGracePeriodWithPayment()
    {
        Status = SubscriptionStatus.Active;
        GracePeriodEndsAt = null;
    }
    
    public void UpdatePaymentMethod(string stripePaymentMethodId, PaymentMethodKind kind, string last4) { ... }
}

public enum SubscriptionStatus { Trial = 1, Active = 2, PastDue = 3, Cancelled = 4, Expired = 5 }
public enum BillingCycle { Monthly = 1, Yearly = 2 }
public enum CancellationReason { UserRequested = 1, PaymentFailed = 2, SuperseededByUpgrade = 3, AdminInitiated = 4, AccountDeleted = 5 }
public enum PaymentMethodKind { CreditCard = 1, BankTransfer = 2, MockTest = 99 }
```

### `Invoice` AR

```csharp
public class Invoice
{
    public Guid Id;
    public string InvoiceNumber;                                // "LT-2026-000123"
    public Guid SubscriberUserId;
    public Guid? SubscriptionId;                                 // null = boost purchase
    public InvoiceKind Kind;                                     // SubscriptionRenewal/Upgrade/Boost/Commission
    
    public Money Subtotal, TaxAmount, Total;
    public decimal? TaxRatePercent;
    public string? TaxJurisdiction;                              // "TR-KDV", "EU-VAT-DE"
    public Money? ProrateAmount;
    public CurrencyCode Currency;
    
    public DateTimeOffset IssuedAt, DueAt;
    public DateTimeOffset? PaidAt, FailedAt, RefundedAt;
    public string? FailureReason;
    public int FailedAttemptCount;
    public Money? RefundedAmount;
    
    public InvoiceStatus Status;
    public string? StripeInvoiceId;
    public string? PdfUrl;                                       // Faz 2 IFileStorage
    
    private readonly List<InvoiceLineItem> _lineItems = new();
    
    public static Invoice IssueForSubscription(...) { ... }
    public static Invoice IssueForBoost(...) { ... }
    public static Invoice IssueForCommission(...) { ... }
    
    public void MarkPaid(DateTimeOffset paidAt, string? stripeChargeId) { /* InvoicePaid */ }
    public void MarkFailed(string reason) { /* InvoiceFailed + grace period */ }
    public void Refund(Money amount, DateTimeOffset refundedAt) { /* InvoiceRefunded */ }
}

public enum InvoiceKind 
{ 
    SubscriptionRenewal = 1, SubscriptionUpgrade = 2, 
    BoostPurchase = 3, CommissionFee = 4 
}

public enum InvoiceStatus { Issued = 1, Paid = 2, Failed = 3, Refunded = 4, Void = 5 }
```

### `BoostPackage` AR (Admin Catalog)

```csharp
public class BoostPackage
{
    public Guid Id;
    public string Code;                                          // "top-row", "showcase", "urgent-label"
    public Translations Name, Description;
    public BoostKind Kind;                                       // Çekince 1 — 2 değer Faz 1
    public int DurationDays;
    public int FeaturedSlots;
    public string? MetadataJson;                                  // urgentBadge: true vs.
    
    private readonly List<BoostPackagePricing> _pricings = new();
    
    public bool IsActive;
    public int DisplayOrder;
    public DateTimeOffset CreatedAt, UpdatedAt;
    
    public static BoostPackage Create(string code, Translations name, BoostKind kind, int durationDays, int featuredSlots) { ... }
    public BoostPackagePricing SetPricing(CurrencyCode currency, Money price) { ... }
    public void Deactivate() { ... }
}

// Çekince 1 — Faz 1 sadece 2 kind
public enum BoostKind 
{ 
    FeaturedListing = 1,         // top-row, urgent-label
    HomepageBanner = 2,           // showcase
    SearchPriority = 3,           // Faz 2
    EmailDigest = 4               // Faz 2
}
```

### `BoostCampaign` AR (Çekince 2 — Tek Listing)

```csharp
public class BoostCampaign
{
    public Guid Id;
    public Guid SubscriberUserId;
    public Guid BoostPackageId;
    public string PackageCodeSnapshot;
    public BoostKind KindSnapshot;
    
    public Guid AppliedListingId;                                // Çekince 2 — TEKİL (eski liste değil)
    
    public DateTimeOffset ActivatedAt, ValidUntil;
    public DateTimeOffset? CancelledAt, ExpiredAt;
    public BoostCampaignStatus Status;
    public Guid InvoiceId;                                       // required (boost mutlaka invoice'a bağlı)
    
    public static BoostCampaign Activate(Guid userId, BoostPackage package, Guid listingId, Guid invoiceId) 
    { 
        /* BoostActivated event */ 
    }
    public void Expire() { /* BoostExpired */ }
    public void Cancel(string? reason) { /* Faz 1 — immediate expire, refund yok */ }
}

public enum BoostCampaignStatus { Active = 1, Expired = 2, Cancelled = 3 }
```

**Bulk purchase:** Frontend 5 listing'e boost almak isterse — backend 5 ayrı BoostCampaign tek invoice altında yaratır.

---

## 3. Child Entities & VO

### `PlanPricing`, `CommissionRule` (Plan AR İçinde)

```csharp
public class PlanPricing
{
    public Guid Id, PlanId;
    public CurrencyCode Currency;
    public Money Monthly, Yearly;
    public decimal? YearlyDiscountPercent;
}

public class CommissionRule
{
    public Guid Id, PlanId;
    public int? CategoryId;                                      // null = plan default
    public decimal RatePercent;                                  // örn. 2.5
    public Money? MinAmount, MaxAmount;
    public DateTimeOffset CreatedAt, UpdatedAt;
}
```

### `PlanFeatures` VO (Patch 2.3 genişletildi)

```csharp
public sealed record PlanFeatures(
    int MaxActiveListings,                  // -1 = unlimited
    int MaxFarms,
    int BoostSlotsIncluded,                  // monthly free boost
    int ListingRefreshesPerMonth,            // YENİ (Patch) — Listings refresh quota
    int AiTranslationsPerMonth,              // YENİ (Patch) — Listings + Messaging shared AI quota
    bool PrioritySupport,
    bool AnalyticsAccess,
    bool BulkImportAllowed);
```

### `PaymentMethod` Entity (Universal — Patch 2.4)

```csharp
public class PaymentMethod
{
    public Guid Id;
    public Guid UserId;                                          // Universal — buyer + seller
    public string StripePaymentMethodId;                         // pm_xxx
    public PaymentMethodKind Kind;
    public string Last4;
    public string? BrandName;                                    // "Visa", "Mastercard"
    public int? ExpiryMonth, ExpiryYear;
    public bool IsDefault;
    public DateTimeOffset CreatedAt;
    public DateTimeOffset? RemovedAt;
}
```

DB constraint: `UNIQUE(user_id, stripe_payment_method_id)`.

Marketplace'in **kendi payment-method endpoint'i yok** — frontend doğrudan `/subscription/me/payment-methods` çağırır.

### `InvoiceLineItem`

```csharp
public class InvoiceLineItem
{
    public Guid Id, InvoiceId;
    public string Description;                                   // "Pro Plan - Aylık (Mayıs 2026)"
    public int Quantity;
    public Money UnitPrice, Subtotal;
}
```

---

## 4. Trial Period (Faz 1 VAR)

**Gerekçe:** Conversion rate ↑, endüstri standardı (Shopify, Stripe), Free tier dar.

**Yapılandırma:**

| Plan | Trial Days |
|---|---|
| Free | 0 |
| Standard | 14 |
| Pro | 14 |
| Enterprise | 0 (sales-led) |

**Bitiminde:**
- AutoRenew=true ve PaymentMethod var → Stripe charge → success: SubscriptionRenewed; failure: grace period
- AutoRenew=false veya PaymentMethod yok → SubscriptionExpired (Free tier'a düş)

**One-trial-per-user kuralı:** User bir kere Pro trial alabilir; ikinci direkt billing.

---

## 5. Prorate Logic

### Upgrade — Immediate Charge

```
Standard (299 TRY/ay) → Pro (599 TRY/ay)
Period: 1 Mayıs - 1 Haziran (31 gün)
Today: 16 Mayıs (15 gün kalan)

remaining_ratio = 15/31 = 0.484
unused_standard_credit = 299 * 0.484 = 144.69 TRY
pro_remaining_cost     = 599 * 0.484 = 289.84 TRY
prorate_charge         = 289.84 - 144.69 = 145.15 TRY

→ Invoice (kind=SubscriptionUpgrade) + Stripe charge
→ Subscription.Upgrade(proPlanId, 599, 145.15)
→ PeriodEnd değişmez (1 Haziran'da regular renewal Pro fiyatından)
```

### Downgrade — Deferred to Period End

```
Pro → Standard
- Subscription.Downgrade(standardPlanId, 299) çağrılır
- PlanId güncellenir, RenewalPrice = 299
- Period sonuna kadar Pro feature'ları aktif
- Period sonunda Standard renewal (299 TRY)
- No refund for downgrade
```

UI'da net belirtilir: "Downgrade 1 Haziran'da yürürlüğe girecek".

---

## 6. Grace Period — InvoiceFailed → 7 Day

```
Stripe webhook → invoice.MarkFailed(reason)
   ↓
Subscription.EnterGracePeriod(7)
   - Status: Active → PastDue
   - GracePeriodEndsAt = now + 7 days
   ↓
Public event: InvoiceFailed
   → Notifications fan-out (email + in-app urgent + SMS)
   → Identity (opsiyonel "Account at risk" flag)
   → Admin escalation_queue insert

Stripe smart retry (internal: 24h, 72h, 7-day)
   - Success → invoice.MarkPaid + Subscription.ExitGracePeriodWithPayment
   - 7 gün sonu no success → cron Subscription.Expire() → SubscriptionExpired
```

---

## 7. Stripe Entegrasyon Placeholder

### Faz 1: Mock + Interface

```csharp
public interface IPaymentProvider
{
    Task<CustomerResult> CreateCustomerAsync(Guid userId, string email, string name, CancellationToken ct);
    Task<SubscriptionResult> CreateSubscriptionAsync(SubscriptionRequest req, CancellationToken ct);
    Task<InvoiceChargeResult> ChargeInvoiceAsync(string customerId, Money amount, string description, CancellationToken ct);
    Task<RefundResult> RefundAsync(string chargeId, Money amount, CancellationToken ct);
    Task<bool> CancelSubscriptionAsync(string subscriptionId, CancellationToken ct);
    Task<PaymentMethodResult> AttachPaymentMethodAsync(string customerId, string paymentMethodToken, CancellationToken ct);
    Task<Result<ChargeOutcome>> ChargeForDealAsync(string customerId, string paymentMethodId, Money amount, string description, string idempotencyKey, CancellationToken ct);
}

// MockPaymentProvider Faz 1 — success default, fail simulation env var
// StripePaymentProvider Faz 2 — Stripe.net SDK gerçek
```

**DI registration env-based:**
```csharp
if (cfg["Subscription:PaymentProvider"] == "Stripe")
    services.AddSingleton<IPaymentProvider, StripePaymentProvider>();
else
    services.AddSingleton<IPaymentProvider, MockPaymentProvider>();
```

### Webhook Endpoint

`POST /subscription/webhooks/payment` — Stripe-Signature header, signature verify (Faz 2 production).

---

## 8. Commission Table — Dapper Flatten Read Model

Karar 5/Subscription Note 2 — AR boundary korunur, read tarafı Dapper flatten.

```csharp
public sealed class CommissionTableReader
{
    public async Task<CommissionTableResponse> GetAsync(CancellationToken ct)
    {
        const string sql = """
            SELECT p.id AS plan_id, p.code, p.tier, p.display_order,
                   cr.id AS rule_id, cr.category_id, cr.rate_percent,
                   cr.min_amount, cr.min_currency, cr.max_amount, cr.max_currency
            FROM subscription.plans p
            LEFT JOIN subscription.commission_rules cr ON cr.plan_id = p.id
            WHERE p.is_active = true
            ORDER BY p.display_order, cr.category_id NULLS FIRST;
            """;
        var rows = await conn.QueryAsync<CommissionRuleRow>(sql);
        return BuildResponse(rows);
    }
}
```

Endpoint `GET /subscription/commission-table` cached 10 min Redis.

---

## 9. Cross-Modül Erişim

### `ISubscriptionReadService` (Shared/)

```csharp
public interface ISubscriptionReadService
{
    Task<SubscriptionSummary?> GetActiveSubscriptionAsync(Guid userId, CancellationToken ct);
    Task<SubscriptionFeatures> GetEffectiveFeaturesAsync(Guid userId, CancellationToken ct);
    Task<decimal> ResolveCommissionRateAsync(Guid sellerUserId, int categoryId, CancellationToken ct);
    Task<int> GetMaxActiveListingsAsync(Guid userId, CancellationToken ct);
    Task<bool> HasActiveBoostAsync(Guid listingId, BoostKind kind, CancellationToken ct);
    Task<IReadOnlyList<BoostCampaignSummary>> ListActiveCampaignsByListingAsync(Guid listingId, CancellationToken ct);
    
    // Patch 2.2 — quota check
    Task<int> GetRefreshQuotaRemainingAsync(Guid userId, CancellationToken ct);
    Task<int> GetTranslationQuotaRemainingAsync(Guid userId, CancellationToken ct);
    
    // Patch 2.2 — payment methods (universal)
    Task<IReadOnlyList<PaymentMethodDto>> GetPaymentMethodsAsync(Guid userId, CancellationToken ct);
    Task<PaymentMethodDto?> GetDefaultPaymentMethodAsync(Guid userId, CancellationToken ct);
}
```

### `ISubscriptionCommands` (Patch 2.1)

```csharp
public interface ISubscriptionCommands
{
    // Quota consume (Listings, Messaging)
    Task<Result> ConsumeRefreshQuotaAsync(Guid userId, CancellationToken ct);
    Task<Result> ConsumeTranslationQuotaAsync(Guid userId, CancellationToken ct);
    
    // Deal payment & refund (Marketplace)
    Task<Result<ChargeOutcome>> ChargeAsync(
        Guid userId, string paymentMethodId, Money amount, 
        string description, string idempotencyKey, CancellationToken ct);
    Task<Result> RefundAsync(string chargeId, Money? amount, string reason, CancellationToken ct);
    
    // Commission (DealCompleted cascade)
    Task<Result> ChargeCommissionAsync(
        Guid sellerUserId, Guid dealId, Money commissionAmount, CancellationToken ct);
}
```

### Seller-Only Guard (Görev 1/F)

```csharp
// CreateSubscriptionValidator
RuleFor(x => x).MustAsync(async (cmd, ct) =>
    await _identity.UserHasRoleAsync(_currentUser.GetUserId()!.Value, "seller", ct))
    .WithMessage("Only verified sellers can subscribe (Faz 1)");

// Faz 2: UserHasAnyRoleAsync(userId, ["seller", "carrier"])
```

### UserDeleted Consumer

```csharp
public sealed class UserDeletedSubscriptionHandler : IConsumer<UserDeleted>
{
    public async Task Consume(...)
    {
        var subs = await _db.Subscriptions
            .Where(s => s.SubscriberUserId == ctx.Message.UserId && s.Status != SubscriptionStatus.Expired)
            .ToListAsync(ct);
        foreach (var sub in subs)
            sub.Cancel(CancellationReason.AccountDeleted, "User account deleted", actorUserId: ctx.Message.UserId);
        // Outstanding invoice → void
    }
}
```

---

## 10. Public Event'ler (14)

| Event | Consumer |
|---|---|
| `SubscriptionActivated` | Listings (limit + boost level), Identity (premium flag?), Notifications, Admin MRR |
| `SubscriptionRenewed` | Notifications, Admin |
| `SubscriptionExpired` | Listings (free tier limit + boost temizle), Notifications, Admin churn |
| `SubscriptionCancelled` | Listings (period sonu demote), Notifications, Admin |
| `SubscriptionUpgraded` | Listings (limit artır), Notifications |
| `SubscriptionDowngraded` | Listings (next period demote), Notifications |
| `InvoiceIssued` | Notifications |
| `InvoicePaid` | Subscription self (lifecycle), Notifications, Admin revenue |
| `InvoiceFailed` | Notifications, Subscription grace, Admin escalation (4 consumer hot path) |
| `InvoiceRefunded` | Notifications, Admin |
| `BoostActivated` | Listings (boost flag apply), Notifications |
| `BoostExpired` | Listings (flag temizle) |
| `BoostCancelled` | Listings, Notifications |
| `CommissionCharged` | Notifications seller, Admin revenue tracking |

---

## 11. API Endpoint Inventory (32)

### Public (4)

`GET /subscription/plans` + `/{code}` + `/commission-table` + `/boost-packages`.

### Authenticated — Subscription (9)

`GET/POST /subscription/me` + `/upgrade` + `/downgrade` + `/cancel` + `/resume` + `GET /invoices?cursor=` + `/{id}` + `POST /payment-method`.

### Authenticated — Boost (4)

`POST /subscription/me/boosts` + `GET /me/boosts?status=` + `/{id}/cancel` + `/{id}` (detail with analytics).

### Webhook (1)

`POST /subscription/webhooks/payment` — Stripe signature.

### Admin — Plans (6)

`GET/POST /admin/subscription/plans` + `PATCH /{id}` + `/pricing` + `/commission-rules` + `/deactivate`.

### Admin — Subscriptions & Invoices (6)

`GET /admin/subscription/subscriptions?cursor=&status=` + `/{id}` + `POST cancel` + `GET /invoices?cursor=&status=` + `/{id}/refund` + `GET /mrr-analytics`.

### Admin — Boost (3)

`POST /admin/subscription/boost-packages` + `PATCH /{id}` + `/deactivate`.

---

## 12. Discovered Backlog

| # | Konu | Hedef |
|---|---|---|
| 104 | Webhook signature verification (Stripe-Signature HMAC Faz 2) | Karar 7 / Security |
| 105 | Invoice PDF generation (QuestPDF, IFileStorage, tax compliance) | Karar 7 / Faz 2 |
| 106 | Tax calculation engine (TR KDV, EU MOSS, US per-state) | Karar 7 / Compliance Faz 2 |
| 107 | Annual billing yearly_discount UX | Karar 5 / Subscription Faz 1 detayı |
| 108 | Promotion / coupon codes Faz 2 | Karar 7 / Marketing |
| 109 | Subscription pause (vs cancel) Faz 2 | Karar 7 / Faz 2 |
| 110 | MRR analytics calculation (Stripe MRR formülü) | Karar 5 / Admin |
| 111 | Failed payment retry strategy | Karar 7 / Payment ops |

---

## 13. Özet Tablo

| Konu | Karar |
|---|---|
| AR sayısı | 5 (Plan, Subscription, Invoice, BoostPackage, BoostCampaign) |
| PaymentMethod | Universal entity (buyer + seller) — Subscription'da |
| Public events | 14 |
| Subscriber naming | `subscriber_user_id` (Görev 1/F future-positive); Faz 1 seller-only guard |
| Trial | 14-day default Pro/Standard; one-trial-per-user |
| Prorate | Upgrade immediate; Downgrade deferred period-end |
| Grace period | 7 day InvoiceFailed → Expired |
| Payment provider | `IPaymentProvider` interface; Faz 1 MockPaymentProvider; Faz 2 StripePaymentProvider |
| Commission | Plan default + per-category override (Faz 2); Dapper flatten read model |
| Boost | 2 kind + 3 paket (Çekince 1); tek listing per campaign (Çekince 2); cross-module analytics (Çekince 3) |
| Endpoint | 32 (4 public + 13 auth + 1 webhook + 15 admin) |
