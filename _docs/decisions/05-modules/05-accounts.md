# Karar 5 / Accounts Modülü (v2)

**Status:** FINAL (v1 → v2 revize)
**Wave:** 3 (parallel with Wave 4 Listings — prod; sequential dev)

## İlişkili Kararlar

- **Üst:** [Karar 2](../02-modules-list.md), [Karar 3a — AR](../03-domain-patterns.md#bölüm-1-aggregate-root-listesi-3a)
- **Patch:** Carrier Çekince 1 (BusinessInfo/BankInfo/Iban Shared'a taşındı); Identity v2 Q3 (VetProfile manuel verification); D6 (Review AR yeni)
- **Frontend:** `frontend-api-inventory.md` Accounts (36 endpoint — seller dashboard, vet profile, buyer stats, follow, reviews, certifications)
- **v1 → v2 değişimi:**
  - D1: Seller Slug routing (SEO public URL `/sellers/{slug}`)
  - D2: Endpoint path pattern `/accounts/me/seller/...` (frontend hizalı)
  - D3: Verification step-per-endpoint (5-step: identity/address/iban/farm-registration/export-approval)
  - D4: BuyerStats endpoint (cross-module aggregator)
  - D5: SellerFollow toggle endpoint set
  - D6: Review AR (2-way: Buyer→Seller + Seller→Buyer)
  - D7: Cover photo endpoint
  - D8: SellerCertification child entity + endpoints
  - Y1: Pre-signed S3 upload URL pattern (büyük dosyalar)
  - Y2: Farm.Purposes [Flags] enum (Breeding/Fattening/Dairy/FeedStorage/TransportHub/Mixed)
  - Y3: HealthRecord/VaccineRecord immutable (void only, edit/delete yasak)
  - Q1: Slug değişimi verified sonrası mümkün, 30 gün eski slug 301 redirect
  - Q2: Self-declared stats (annualMilkLiters, staffCount) + computed (listingCount, avgRating)
  - Q3: Vet'in Farm'ı YOK (Farm.SellerId zorunlu FK, validator)
  - Identity v2 Q3: VetProfile new AR — manuel admin verification, role grant sonrası

---

## 1. Modülün Rolü ve Sınırları

### Sahip

| Konsept | Sahiplik |
|---|---|
| Seller business profile + onboarding state machine | Accounts |
| Farm geographic + operational profile | Accounts |
| **VetProfile** professional profile + verification | Accounts (yeni v2) |
| Animal health & vaccine records (immutable) | Accounts (Farm AR child) |
| Verification document upload + admin review | Accounts |
| IBAN + bank account info (Iban VO Shared'a taşındı) | Accounts |
| **ÇKS** (Çiftçi Kayıt Sistemi) registration number | Accounts (Seller field) |
| Ministry data sync (Faz 2 placeholder) | Accounts |
| **Review** AR (Buyer↔Seller bidirectional) | Accounts (v2 yeni) |
| **SellerFollow** junction (UserId follows SellerId) | Accounts |
| **SellerCertification** child entity (Faz 1 D8) | Accounts (Seller AR child) |
| Slug routing + 301 redirect 30 gün | Accounts (Seller, Carrier'da paralel) |

### Sahip Olmayan

| Konsept | Sahibi |
|---|---|
| User identity + auth + KVKK | Identity |
| Carrier persona | Carrier |
| Subscription/billing | Subscription |
| Listing CRUD (Farm/HealthRecord reference verilir) | Listings |
| Document physical storage | Shared IFileStorage → MinIO |

---

## 2. Aggregate Roots

### `Seller` AR

```csharp
public class Seller
{
    public Guid Id { get; private set; }                       // Guid v7
    public Guid UserId { get; private set; }                   // ID-ref Identity, UNIQUE
    public string Slug { get; private set; }                   // public URL — değişebilir (Q1)
    public string? PreviousSlug { get; private set; }          // 30 gün 301 redirect (Q1)
    public DateTimeOffset? PreviousSlugExpiresAt { get; private set; }
    
    public BusinessInfo Business { get; private set; }         // Shared VO
    public BankInfo? Bank { get; private set; }                // Shared VO
    public string? CksNumber { get; private set; }              // ÇKS — TR-spesifik
    public string? TaxOffice { get; private set; }
    public SellerType Type { get; private set; }                // Producer | Trader
    
    // Self-declared operational stats (Q2)
    public int? AnnualMilkLitersDeclared { get; private set; }
    public int? StaffCountDeclared { get; private set; }
    public string? OperationalSince { get; private set; }       // "2008-bugün"
    
    public string? AvatarUrl { get; private set; }              // D7
    public string? CoverPhotoUrl { get; private set; }          // D7 yeni
    
    public OnboardingStatus OnboardingStatus { get; private set; }
    public DateTimeOffset? VerifiedAt { get; private set; }
    public Guid? VerifiedByUserId { get; private set; }
    public DateTimeOffset? SuspendedAt { get; private set; }
    public string? SuspendedReason { get; private set; }
    
    public DateTimeOffset CreatedAt, UpdatedAt;
    
    // Child collections
    private readonly List<SellerDocument> _documents = new();
    private readonly List<SellerVerification> _verifications = new();
    private readonly List<SellerCertification> _certifications = new();   // D8
    
    // Factories
    public static Seller StartOnboarding(Guid userId, AccountType accountType, BusinessInfo info, string proposedSlug)
    {
        if (accountType != AccountType.Producer && accountType != AccountType.Trader)
            throw new DomainException("Only Producer or Trader can become Seller");
        // ...
    }
    
    // Verification step-per-endpoint (D3 — 5 step)
    public SellerVerification SubmitIdentityVerification(IReadOnlyList<string> docUrls) { ... }
    public SellerVerification SubmitAddressVerification(string docUrl) { ... }
    public SellerVerification SubmitIbanVerification(string holderName, Iban iban, string bankName, string? docUrl) { ... }
    public SellerVerification SubmitFarmRegistrationVerification(string? cksNumber, string docUrl) { ... }
    public SellerVerification SubmitExportApprovalVerification(string docUrl, string? scopeNote) { ... }
    
    public void SubmitForVerification()
    {
        // Umbrella — tüm zorunlu verification step'ler tamamlandığında
        var requiredTypes = Type switch
        {
            SellerType.Producer => new[] 
            { 
                VerificationType.Identity, VerificationType.Address, 
                VerificationType.Bank, VerificationType.FarmRegistration 
            },
            SellerType.Trader => new[] 
            { 
                VerificationType.Identity, VerificationType.Address, VerificationType.Bank 
            },
            _ => throw new InvalidOperationException()
        };
        
        foreach (var rt in requiredTypes)
            if (!_verifications.Any(v => v.Type == rt && v.Status != VerificationStatus.Rejected))
                throw new DomainException($"Verification {rt} missing");
        
        OnboardingStatus = OnboardingStatus.Submitted;
        // Public event: SellerOnboardingSubmitted
    }
    
    // Slug Q1 — 30 day redirect
    public void UpdateSlug(string newSlug)
    {
        if (OnboardingStatus != OnboardingStatus.Verified)
            throw new DomainException("Slug değişikliği sadece Verified Seller için");
        var normalized = SlugHelper.Normalize(newSlug);
        if (normalized == Slug) return;
        
        PreviousSlug = Slug;
        PreviousSlugExpiresAt = DateTimeOffset.UtcNow.AddDays(30);
        Slug = normalized;
        Touch();
    }
    
    // Cover photo (D7)
    public void UpdateAvatar(string url) { ... }
    public void UpdateCoverPhoto(string url) { ... }
    public void RemoveCoverPhoto() { ... }
    
    // Operational stats (Q2)
    public void UpdateOperationalStats(int? milkLiters, int? staff, string? since) { ... }
    
    // Certifications (D8)
    public SellerCertification AddCertification(int certificationTypeId, string certUrl, DateTimeOffset? issuedAt, DateTimeOffset? expiresAt)
    {
        if (_certifications.Any(c => c.CertificationTypeId == certificationTypeId 
                                      && c.Status == CertReviewStatus.Pending))
            throw new DomainException("Aynı sertifika tipi için pending kayıt var");
        var cert = new SellerCertification(...);
        _certifications.Add(cert);
        return cert;
    }
    public void RemoveCertification(Guid certificationId, Guid actorUserId)
    {
        var cert = _certifications.First(c => c.Id == certificationId);
        if (cert.Status == CertReviewStatus.Verified)
            throw new DomainException("Verified sertifika silinemez, admin destek");
        _certifications.Remove(cert);
    }
    
    // ÇKS
    public void AssignCksNumber(string cks)
    {
        if (Type != SellerType.Producer)
            throw new DomainException("ÇKS only applies to Producer sellers");
        if (!Regex.IsMatch(cks, @"^\d{7,12}$"))
            throw new DomainException("Invalid ÇKS format");
        CksNumber = cks;
    }
    
    public void UpdateBankInfo(BankInfo bank) { /* IBAN validation Shared VO içinde */ }
    
    // Lifecycle
    public void Verify(Guid actorAdminUserId) { /* Public event: SellerVerified */ }
    public void Reject(Guid actorAdminUserId, string reason) { /* Internal event */ }
    public void Suspend(string reason, Guid actorAdminUserId) { /* Public event: SellerSuspended — 5 consumer cascade */ }
    public void Reactivate(Guid actorAdminUserId) { /* Public event: SellerReactivated */ }
}

public enum SellerType { Producer = 1, Trader = 2 }
```

### `Farm` AR

```csharp
public class Farm
{
    public Guid Id { get; private set; }
    public Guid SellerId { get; private set; }                  // ID-ref Seller (Vet'in Farm'ı YOK — Q3)
    public string Name { get; private set; }
    public FarmType Type { get; private set; }
    public FarmLocation Location { get; private set; }          // VO — PostGIS Point + LocationId
    public int? HerdSize { get; private set; }
    public bool IsActive { get; private set; }
    
    // Y2 — Multi-tag purpose
    public FarmPurpose Purposes { get; private set; }           // [Flags]
    
    public DateTimeOffset CreatedAt, UpdatedAt;
    
    // Child collections — IMMUTABLE (Y3)
    private readonly List<HealthRecord> _healthRecords = new();
    private readonly List<VaccineRecord> _vaccineRecords = new();
    
    public static Farm Create(Guid sellerId, string name, FarmType type, FarmLocation location, FarmPurpose purposes)
    {
        if (purposes == FarmPurpose.None) throw new DomainException("En az 1 purpose");
        return new Farm { /* ... */ };
    }
    
    public bool HasPurpose(FarmPurpose p) => (Purposes & p) == p;
    public bool CanListAnimals =>
        Purposes.HasFlag(FarmPurpose.Breeding) 
        || Purposes.HasFlag(FarmPurpose.Fattening)
        || Purposes.HasFlag(FarmPurpose.Dairy)
        || Purposes.HasFlag(FarmPurpose.Mixed);
    public bool CanHaveHealthRecords => CanListAnimals;
    
    public HealthRecord AddHealthRecord(...)
    {
        if (!CanHaveHealthRecords) 
            throw new DomainException("FeedStorage/TransportHub farm'a health record eklenemez");
        if (!IsActive) throw new DomainException();
        var record = new HealthRecord(...);
        _healthRecords.Add(record);
        return record;
    }
    
    public VaccineRecord AddVaccineRecord(...) { /* benzer */ }
    
    // Y3 — IMMUTABLE: edit/delete YOK
    public void VoidHealthRecord(Guid recordId, string reason, Guid actorUserId) { /* IsVoided=true */ }
    public void VoidVaccineRecord(Guid recordId, string reason, Guid actorUserId) { /* IsVoided=true */ }
    
    public void UpdateLocation(FarmLocation newLocation) { ... }
    public void UpdateHerdSize(int? size) { ... }
    public void UpdatePurposes(FarmPurpose newPurposes) { /* validate != None */ }
    public void Deactivate() { /* event tetikler: listing'leri pause */ }
    public void Reactivate() { ... }
}

[Flags]
public enum FarmPurpose
{
    None = 0,
    Breeding = 1,
    Fattening = 2,
    Dairy = 4,
    FeedStorage = 8,
    TransportHub = 16,
    Mixed = 32
}

public enum FarmType
{
    Cattle = 1, Sheep = 2, Goat = 3, Poultry = 4,
    Beekeeping = 5, Mixed = 6, Other = 99
}
```

### `VetProfile` AR (v2 YENİ — Identity Q3)

```csharp
public class VetProfile
{
    public Guid Id { get; private set; }
    public Guid UserId { get; private set; }                    // ID-ref Identity
    
    public string LicenseNumber { get; private set; }            // TVHB (Türkiye Veteriner Hekimleri Birliği)
    public string? Specialization { get; private set; }          // "Büyükbaş", "Küçükbaş", ...
    public string ProfessionalName { get; private set; }         // "Vet. Hek. Dr. ..."
    public string? Bio { get; private set; }
    public string? ClinicName { get; private set; }
    public Address? ClinicAddress { get; private set; }          // Shared VO
    public IReadOnlyList<int> ServiceLocationIds { get; private set; }   // Catalog Location ref
    
    public VetVerificationStatus VerificationStatus { get; private set; }
    public DateTimeOffset? VerifiedAt { get; private set; }
    public Guid? VerifiedByUserId { get; private set; }
    public DateTimeOffset? SuspendedAt { get; private set; }
    public string? SuspendedReason { get; private set; }
    
    public DateTimeOffset CreatedAt, UpdatedAt;
    
    private readonly List<VetDocument> _documents = new();
    
    public static VetProfile StartOnboarding(Guid userId, string licenseNumber, string professionalName, string? specialization)
    {
        if (!Regex.IsMatch(licenseNumber, @"^\d{4,8}$"))
            throw new DomainException("TVHB license number must be 4-8 digits");
        return new VetProfile { /* status=Draft */ };
    }
    
    public VetDocument UploadDocument(VetDocumentType type, string url, string fileName, long size) { ... }
    
    public void SubmitForVerification()
    {
        if (VerificationStatus != VetVerificationStatus.Draft) throw new DomainException();
        if (!_documents.Any(d => d.Type == VetDocumentType.Diploma))
            throw new DomainException("Diploma required");
        VerificationStatus = VetVerificationStatus.Submitted;
        // Internal event (Faz 1) / Public (Faz 2): VetOnboardingSubmitted
    }
    
    public void Verify(Guid actorAdminUserId)
    {
        VerificationStatus = VetVerificationStatus.Verified;
        VerifiedAt = DateTimeOffset.UtcNow;
        VerifiedByUserId = actorAdminUserId;
        // Public event: VetVerified — Identity consumer 'vet' role grant
    }
    
    public void Reject(Guid actorAdminUserId, string reason) { ... }
    public void Suspend(string reason, Guid actorAdminUserId) { /* Public event: VetSuspended */ }
    public void Reactivate(Guid actorAdminUserId) { /* Public event: VetReactivated */ }
    
    public void UpdateSpecialization(string? value) { ... }
    public void UpdateClinicInfo(string? name, Address? address) { ... }
    public void UpdateServiceLocations(IReadOnlyList<int> locationIds) { ... }
}

public enum VetVerificationStatus
{
    Draft = 1, Submitted = 2, UnderReview = 3,
    Verified = 4, Rejected = 5, Suspended = 6
}
```

### `Review` AR (v2 YENİ — D6)

```csharp
public class Review
{
    public Guid Id { get; private set; }
    public Guid DealId { get; private set; }                    // ID-ref Marketplace, IMMUTABLE
    public Guid ReviewerUserId { get; private set; }
    public Guid ReviewedUserId { get; private set; }
    public ReviewDirection Direction { get; private set; }       // BuyerToSeller | SellerToBuyer
    public byte Rating { get; private set; }                     // 1-5
    public string? Title { get; private set; }
    public string? Body { get; private set; }
    public ReviewReply? SellerReply { get; private set; }       // VO içeride
    public DateTimeOffset CreatedAt, UpdatedAt;
    
    public static Review Write(
        Guid dealId, Guid reviewerUserId, Guid reviewedUserId,
        ReviewDirection direction, byte rating, string? title, string? body)
    {
        if (rating < 1 || rating > 5) throw new DomainException("1-5");
        if (reviewerUserId == reviewedUserId) throw new DomainException("Self-review yasak");
        // Cross-modül validator: Deal.Status=Completed, participant check
        return new Review { Id = Guid.CreateVersion7(), /* ... */ };
        // Public event: ReviewWritten
    }
    
    public void AddSellerReply(string body, Guid actorUserId)
    {
        if (Direction != ReviewDirection.BuyerToSeller)
            throw new DomainException("Reply sadece BuyerToSeller review'a");
        if (actorUserId != ReviewedUserId)
            throw new DomainException("Sadece review edilen seller cevap yazabilir");
        if (SellerReply is not null)
            throw new DomainException("Tek reply per review");
        SellerReply = new ReviewReply(body.Trim(), DateTimeOffset.UtcNow);
    }
}

public enum ReviewDirection { BuyerToSeller = 1, SellerToBuyer = 2 }

public sealed record ReviewReply(string Body, DateTimeOffset ReplyAt);
```

DB constraint: `UNIQUE(deal_id, direction)` — aynı deal başına 1 review per direction.

---

## 3. Child Entities

### `SellerDocument`, `SellerVerification` (Seller AR İçinde)

```csharp
public class SellerDocument
{
    public Guid Id, SellerId;
    public DocumentType Type;                                    // Identity/Tax/CKS/Bank/Chamber/etc.
    public string FileUrl, FileName;
    public long SizeBytes;
    public DocumentReviewStatus ReviewStatus;
    public string? ReviewerComment;
    public Guid? ReviewedByUserId;
    public DateTimeOffset? ReviewedAt;
    public DateTimeOffset UploadedAt;
}

public class SellerVerification
{
    public Guid Id, SellerId;
    public VerificationType Type;                                // Identity/Address/Bank/FarmRegistration/ExportApproval/CksMinistry
    public VerificationStatus Status;
    public string? VerifierNote;
    public Guid? VerifiedByUserId;
    public DateTimeOffset? VerifiedAt;
}

public enum VerificationType
{
    Identity = 1, Address = 2, Bank = 3, 
    FarmRegistration = 4, ExportApproval = 5,
    CksMinistry = 6        // Faz 2 — Bakanlık otomatik sync
}

public enum VerificationStatus { Pending = 1, Verified = 2, Rejected = 3, Expired = 4 }
```

### `SellerCertification` (D8 — Seller AR İçinde)

```csharp
public class SellerCertification
{
    public Guid Id, SellerId;
    public int CertificationTypeId;                              // Catalog reference
    public string CertificateUrl;
    public DateTimeOffset? IssuedAt, ExpiresAt;
    public CertReviewStatus Status;
    public Guid? ReviewedByUserId;
    public DateTimeOffset? ReviewedAt;
    public string? ReviewerNote;
    public DateTimeOffset UploadedAt;
}

public enum CertReviewStatus { Pending = 1, Verified = 2, Rejected = 3, Expired = 4 }
```

Cron daily: `Verified && ExpiresAt < now` → `Expired` + `SellerCertificationExpired` Public event.

### `HealthRecord`, `VaccineRecord` (Farm AR İçinde — IMMUTABLE)

```csharp
public class HealthRecord
{
    public Guid Id, FarmId;
    public string AnimalIdentifier;                              // küpe no
    public HealthCheckType CheckType;
    public DateTimeOffset CheckDate;
    public Guid? VetUserId;                                      // ID-ref Identity (Vet role)
    public string Findings;
    public string? AttachedDocumentUrl;
    public bool IsVoided;                                         // immutable, void only
    public string? VoidReason;
    public DateTimeOffset RecordedAt;
}

public enum HealthCheckType { Routine = 1, Disease = 2, Quarantine = 3, PreSale = 4, Test = 5 }

public class VaccineRecord
{
    public Guid Id, FarmId;
    public string AnimalIdentifier;
    public string VaccineCode;                                   // "FMD" (şap), "BRUC" (brusella)
    public DateTimeOffset AdministeredAt;
    public Guid? VetUserId;
    public string? BatchNumber, ManufacturerLot;
    public DateTimeOffset? NextDueDate;
    public bool IsVoided;
    public string? VoidReason;
    public DateTimeOffset RecordedAt;
}
```

Endpoint pattern (Y3): `POST /me/farms/{id}/health-records/{recordId}/void` (PATCH/DELETE YOK).

### `VetDocument` (VetProfile AR İçinde)

```csharp
public class VetDocument
{
    public Guid Id, VetProfileId;
    public VetDocumentType Type;                                 // Diploma/TvhbLicense/SpecializationCert/IdentityFront/IdentityBack
    public string FileUrl, FileName;
    public long SizeBytes;
    public DocumentReviewStatus ReviewStatus;
    public Guid? ReviewedByUserId;
    public DateTimeOffset? ReviewedAt;
    public DateTimeOffset UploadedAt;
}
```

### `SellerFollow` (D5 — Junction, AR Değil)

```csharp
public class SellerFollow
{
    public Guid Id;
    public Guid FollowerUserId;
    public Guid SellerId;
    public DateTimeOffset FollowedAt;
}
```

DB: `UNIQUE(follower_user_id, seller_id)`. Idempotent toggle.

---

## 4. Cross-Modül Erişim

### `IAccountsReadService` (Shared/)

```csharp
public interface IAccountsReadService
{
    // Seller
    Task<Guid?> GetSellerIdByUserIdAsync(Guid userId, CancellationToken ct);
    Task<SellerSummary?> GetSellerSummaryAsync(Guid sellerId, CancellationToken ct);
    Task<SellerSummary?> GetSellerSummaryByUserIdAsync(Guid userId, CancellationToken ct);
    Task<bool> IsActiveSellerAsync(Guid userId, CancellationToken ct);
    
    // Farm
    Task<IReadOnlyList<FarmSummary>> ListFarmsBySellerAsync(Guid sellerId, CancellationToken ct);
    Task<FarmSummary?> GetFarmSummaryAsync(Guid farmId, CancellationToken ct);
    Task<bool> FarmBelongsToSellerAsync(Guid farmId, Guid sellerId, CancellationToken ct);
    Task<bool> FarmCanListAnimalsAsync(Guid farmId, CancellationToken ct);   // Y2 — Listings validator
    
    // Vet
    Task<Guid?> GetVetProfileIdByUserIdAsync(Guid userId, CancellationToken ct);
    Task<VetSummary?> GetVetSummaryAsync(Guid vetProfileId, CancellationToken ct);
    Task<bool> IsActiveVetAsync(Guid userId, CancellationToken ct);
    
    // HealthRecord/Vaccine (Listings "verified health" badge)
    Task<HealthRecordSummary?> GetLatestHealthCheckAsync(string animalIdentifier, Guid farmId, CancellationToken ct);
    Task<IReadOnlyList<VaccineRecordSummary>> ListRecentVaccinesAsync(string animalIdentifier, Guid farmId, CancellationToken ct);
    
    // SellerCertification (D8 — Listings "verified_breeding" badge)
    Task<bool> SellerHasCertificationAsync(Guid sellerId, string certCode, CancellationToken ct);
    Task<IReadOnlyList<SellerCertificationSummary>> ListSellerCertificationsAsync(Guid sellerId, CancellationToken ct);
}
```

### `IAdminSellerCommands` + `IAdminVetCommands`

```csharp
public interface IAdminSellerCommands
{
    Task<Result> VerifyAsync(Guid sellerId, Guid actorId, CancellationToken ct);
    Task<Result> RejectAsync(Guid sellerId, string reason, Guid actorId, CancellationToken ct);
    Task<Result> SuspendAsync(Guid sellerId, string reason, Guid actorId, CancellationToken ct);
    Task<Result> ReactivateAsync(Guid sellerId, Guid actorId, CancellationToken ct);
    Task<Result> ReviewDocumentAsync(Guid documentId, DocumentReviewStatus status, string? comment, Guid actorId, CancellationToken ct);
    Task<Result> ReviewCertificationAsync(Guid certificationId, CertReviewStatus status, Guid actorId, CancellationToken ct);
}

public interface IAdminVetCommands
{
    Task<Result> VerifyAsync(Guid vetProfileId, Guid actorId, CancellationToken ct);
    Task<Result> RejectAsync(Guid vetProfileId, string reason, Guid actorId, CancellationToken ct);
    Task<Result> SuspendAsync(Guid vetProfileId, string reason, Guid actorId, CancellationToken ct);
    Task<Result> ReactivateAsync(Guid vetProfileId, Guid actorId, CancellationToken ct);
    Task<Result> ReviewDocumentAsync(Guid documentId, DocumentReviewStatus status, Guid actorId, CancellationToken ct);
}
```

### `IDataExportContributor` Implementation

```csharp
public sealed class AccountsDataExportContributor : IDataExportContributor
{
    public string ModuleName => "accounts";
    
    public async Task<object> ExportAsync(Guid userId, CancellationToken ct)
    {
        var seller = await _db.Sellers
            .Include(s => s.Documents).Include(s => s.Verifications).Include(s => s.Certifications)
            .FirstOrDefaultAsync(s => s.UserId == userId, ct);
        
        var farms = seller is null ? new() : await _db.Farms
            .Include(f => f.HealthRecords).Include(f => f.VaccineRecords)
            .Where(f => f.SellerId == seller.Id).ToListAsync(ct);
        
        var vet = await _db.VetProfiles
            .Include(v => v.Documents).FirstOrDefaultAsync(v => v.UserId == userId, ct);
        
        var reviews = await _db.Reviews
            .Where(r => r.ReviewerUserId == userId || r.ReviewedUserId == userId)
            .ToListAsync(ct);
        
        return new { Seller = ..., Farms = ..., VetProfile = ..., Reviews = ... };
    }
}
```

---

## 5. Public Event'ler (11)

| Event | Producer | Consumer |
|---|---|---|
| `SellerOnboardingSubmitted` | Accounts | Notifications (moderator queue), Admin |
| `SellerVerified` | Accounts | Identity (seller role grant), Notifications |
| `SellerSuspended` | Accounts | Listings (auto-pause), Marketplace (cancel offers), Subscription (pause), Notifications, Admin |
| `SellerReactivated` | Accounts | Notifications |
| `VetVerified` (v2) | Accounts | Identity (vet role grant), Notifications |
| `VetSuspended` (v2) | Accounts | Identity (role revoke), Notifications |
| `VetReactivated` (v2) | Accounts | Identity, Notifications |
| `ReviewWritten` (v2) | Accounts | Notifications (review edilen kullanıcıya), Admin (rating aggregate) |
| `SellerCertificationExpired` (D8) | Accounts | Notifications (seller uyarı) |
| `SellerSubscriptionActivated` (eski) | → Subscription'a taşındı | — |
| `SellerSubscriptionExpired` (eski) | → Subscription'a taşındı | — |
| `SellerBoostPurchased` (eski) | → Subscription'a taşındı | — |

**Internal:** SellerOnboardingStarted, SellerDocumentUploaded/Approved/Rejected, SellerSlugChanged.

---

## 6. API Endpoint Inventory (73)

### Authenticated — Seller (20)

`POST/GET/PATCH /accounts/me/seller`, `/business-info`, `/bank-info`, `/cks`, `/operational-stats`, `/slug`, `/avatar`, `/cover-photo`, `/upload-url`, 5× `/verifications/{type}`, `/submit`, `/verification-status`, `/certifications`.

### Authenticated — Farm (12)

`POST/GET/PATCH /accounts/me/farms` + `/{id}` + `/deactivate` + `/reactivate` + `/health-records` (GET/POST/void) + `/vaccine-records` (GET/POST/void).

### Authenticated — Vet (10)

`POST/GET/PATCH /accounts/me/vet` + `/specialization` + `/clinic` + `/service-locations` + `/upload-url` + `/submit` + `/verification-status`.

### Authenticated — Buyer & Social (7)

| Method | Path |
|---|---|
| GET | `/accounts/me/buyer-stats` (D4 — cross-module aggregator) |
| GET | `/accounts/me/followed-sellers` |
| POST | `/accounts/sellers/{id}/follow` |
| DELETE | `/accounts/sellers/{id}/follow` |
| GET | `/accounts/me/written-reviews` |
| GET | `/accounts/me/received-reviews?direction=` |
| POST | `/accounts/sellers/{slugOrId}/reviews` |
| POST | `/accounts/reviews/{reviewId}/reply` |

### Public (8)

`GET /accounts/sellers?cursor=&filters=` + `/{slugOrId}` + `/certifications` + `/reviews` + `/farms` + `/vets?cursor=` + `/vets/{slugOrId}` + `/sellers/{slugOrId}/follower-count`.

### Admin (16)

Seller (8): list/queue/verify/reject/suspend/reactivate/document-review/certification-review.
Vet (6): aynı pattern.
Verification cross-aggregate (2): all-verifications-list, single-detail.

---

## 7. Özel Konular

### BuyerStats (D4) — Cross-Module Aggregator

```csharp
public sealed record BuyerStatsResponse(
    int FavoriteCount,                    // IListingsReadService
    int ActiveOfferCount,                  // IMarketplaceReadService
    int CompletedDealCount,                // IMarketplaceReadService
    Money TotalSpent,
    decimal? AvgRating,                    // Reviews aggregate
    int ReviewCount,
    decimal? RepeatPurchasePercent,        // IMarketplaceReadService
    DateTimeOffset MemberSince);            // IIdentityReadService.CreatedAt
```

Paralel cross-module read (premature abstraction yapma — direct service calls).

### Pre-Signed Upload URL (Y1)

```csharp
POST /accounts/me/seller/upload-url
   { kind, filename, contentType, sizeBytes }
   → { uploadUrl, fileUrl, expiresAt }
```

Bucket/key mapping per kind:

| Kind | Bucket | Max Size | TTL |
|---|---|---|---|
| `seller-document` | `seller-docs` | 20 MB | 15 dk |
| `seller-certification` | `seller-certs` | 10 MB | 15 dk |
| `vet-document` | `vet-docs` | 20 MB | 15 dk |
| `cover-photo` | `cover-photos` | 5 MB | 10 dk |
| `avatar` | `avatars` | 2 MB | 10 dk |

≤ 2 MB → multipart direct (mevcut endpoint); > 2 MB → pre-signed.

### Slug Strategy (Q1)

Verified Seller slug değiştirebilir, 30 gün eski slug 301 redirect. Daily cron `PreviousSlug` 30+ gün eski → NULL.

Rate limit: 1 user max 30 günde 1 slug değişimi.

Public URL: `GET /accounts/sellers/{slugOrId}` — slug match → previous → ID fallback → 404.

### Ministry Sync Placeholder (Faz 2)

```
accounts.ministry_sync_log table (boş Faz 1)
SellerVerification.Type=CksMinistry (Faz 1 hep Pending)
vaccine_records.source enum (Manual / MinistryHbs Faz 2)
Quartz job MinistrySyncJob (disabled Faz 1)
```

Cross-modül: `IIdentityReadService.HasActiveConsentAsync(userId, MinistryDataShare)` check — KVKK gerekli.

---

## 8. Discovered Backlog

| # | Konu | Hedef |
|---|---|---|
| 10 | Address taşıma kararı (Faz 2 ikinci modül kullanımı) | Karar 5 / Faz 2 |
| 78 | DisplayName Seller enrichment kuralı | Karar 5 / Accounts |
| 82 | Karar 3a/Görev 4 AR sayım güncelle 28→30 (kapandı) | ✓ |
| 83 | Görev 3 Communication Matrix Vet event'leri (kapandı) | ✓ |
| 84 | Bakanlık HBS sync provider Faz 2 | Karar 7 / External Faz 2 |
| 85 | TVHB license verification API Faz 2 | Karar 7 / External Faz 2 |
| 86 | ChamberVerification (Esnaf/Ticaret Odası) Faz 1 manuel | Karar 5 / Accounts ek |
| 87 | HealthRecord/VaccineRecord ICVD/SNOMED coding Faz 2 | Karar 7 / Standards Faz 2 |
| 88 | Farm location PostGIS GIST index | Karar 4e detay |
| 89 | VetProfile public dizin sıralama (service location proximity + verified-first) | Karar 5 / Accounts |
| 90 | Slug uniqueness scope (per-module global) | Karar 5 / Accounts |
| 91 | SlugHelper Türkçe transliterasyon test | Karar 7 / Process |
| 92 | Cover photo image processing pipeline (SixLabors) | Karar 7 / Operations |
| 93 | Review fraud detection Faz 2 | Karar 7 / Trust&Safety |
| 94 | Followers aggregate cache (high-write counter) | Karar 7 / Performance |
| 95 | Pre-signed URL audit | Karar 7 / Observability |
| 96 | Cross-module Listings → IAccountsReadService.FarmCanListAnimalsAsync TTL | Karar 5 / Listings + Faz 2 cache |

---

## 9. Özet Tablo

| Konu | Karar |
|---|---|
| AR sayısı | 4 (Seller, Farm, **VetProfile**, **Review**) |
| Public events | 11 |
| Slug routing | Verified sonrası değişebilir, 30 gün 301 redirect (Q1) |
| Verification flow | 5-step per-endpoint (Identity/Address/Bank/FarmRegistration/ExportApproval) — Producer 4 zorunlu, Trader 3 |
| FarmPurpose | [Flags] (Breeding/Fattening/Dairy/FeedStorage/TransportHub/Mixed) — Listings validator |
| Health/Vaccine | IMMUTABLE — POST + void only (PATCH/DELETE yasak Y3) |
| Vet | Manual admin verification (Identity Q3) — AccountType=Vet self-declared, role grant Accounts.VetVerified sonrası |
| Review | 2-yön (Buyer→Seller + Seller→Buyer), DealId immutable, deal.Completed validator, UNIQUE(deal,direction) |
| Pre-signed URL | > 2 MB için MinIO direct upload pattern (5 kind: docs/certs/vet-docs/cover/avatar) |
| Endpoint | 73 (20 Seller + 12 Farm + 10 Vet + 7 Buyer/Social + 8 Public + 16 Admin) |
