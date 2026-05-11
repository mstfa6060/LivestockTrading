# Karar 5 / Carrier Modülü

**Status:** FINAL
**Wave:** 5 (parallel with Subscription)

## İlişkili Kararlar

- **Üst:** [Karar 2 revize](../02-modules-list.md) — Carrier Accounts'tan ayrıldı
- **Patch:** Carrier Çekince 1 (BusinessInfo/BankInfo/Iban Shared'a); Çekince 2 (slug per-module unique); Çekince 3 (VehicleType 7 değer frontend hizalı); Çekince 4 (InitDraft minimal pattern)
- **Frontend:** `frontend-api-inventory.md` Carrier (18 endpoint — public dizin, onboarding, fleet, zones, rates, shipments)

---

## 1. Modülün Rolü ve Sınırları

### Sahip

| Konsept | Sahiplik |
|---|---|
| Carrier persona (firma + onboarding) | Carrier |
| Fleet (Vehicle, Driver — Faz 1 internal employee) | Carrier |
| ServiceArea (PostGIS MultiPolygon coverage) | Carrier |
| Zone × Rate matrix (8 region × livestock type pricing) | Carrier |
| Shipment lifecycle (Pending → Delivered) | Carrier |
| CarrierOffer (shipment proposal) | Carrier |
| Auto-toggle rules (rate eksikse offer kabul etmiyor) | Carrier |

### Sahip Olmayan

| Konsept | Sahibi |
|---|---|
| Carrier'ın User'ı | Identity |
| Carrier subscription | Subscription (Faz 1 muhtemelen no-op) |
| Deal lifecycle | Marketplace (Carrier shipment phase'inde devreye) |
| Driver mobile login (Faz 2) | Faz 2 Identity yükseltme |

---

## 2. Aggregate Roots

### `Carrier` AR

```csharp
public class Carrier
{
    public Guid Id { get; private set; }
    public Guid UserId { get; private set; }
    public string Slug { get; private set; }                      // Çekince 2 — per-module unique
    public string? PreviousSlug { get; private set; }
    public DateTimeOffset? PreviousSlugExpiresAt { get; private set; }
    
    public BusinessInfo Business { get; private set; }            // Shared VO (Çekince 1)
    public BankInfo? Bank { get; private set; }
    public string? AvatarUrl, CoverPhotoUrl;
    
    public OnboardingStatus OnboardingStatus { get; private set; }
    public DateTimeOffset? VerifiedAt, SuspendedAt;
    public Guid? VerifiedByUserId;
    public string? SuspendedReason;
    
    public bool AcceptsAutoAssignment { get; private set; }       // auto-toggle master switch
    public DateTimeOffset CreatedAt, UpdatedAt;
    
    private readonly List<CarrierDocument> _documents = new();
    private readonly List<CarrierServiceArea> _serviceAreas = new();
    private readonly List<CarrierZone> _zones = new();
    private readonly List<CarrierRate> _rates = new();
    private readonly List<Vehicle> _vehicles = new();
    private readonly List<Driver> _drivers = new();
    
    // Çekince 4 — Minimal init pattern (frontend wizard step 1)
    public static Carrier InitDraft(Guid userId, string companyName, string taxNumber, CountryCode country, string city, string? region)
    {
        var partialBusiness = BusinessInfo.PartialInit(
            legalName: companyName,
            taxNumber: TaxNumber.Parse(taxNumber, country),
            country: country, city: city, region: region);
        
        return new Carrier
        {
            Id = Guid.CreateVersion7(),
            UserId = userId,
            Business = partialBusiness,
            Slug = SlugHelper.Normalize(companyName),
            OnboardingStatus = OnboardingStatus.Draft,
            AcceptsAutoAssignment = false,
            CreatedAt = DateTimeOffset.UtcNow,
        };
    }
    
    public void UpdateBusinessInfo(BusinessInfo full) { /* full required only at Submit time */ }
    
    public void SubmitForVerification()
    {
        if (string.IsNullOrWhiteSpace(Business.TaxOffice)) throw new DomainException("TaxOffice required");
        if (Business.RegisteredAddress?.AddressLine1.Length is 0 or null) throw new DomainException("Address required");
        if (Business.ContactEmail is null) throw new DomainException("ContactEmail required");
        if (Business.ContactPhone is null) throw new DomainException("ContactPhone required");
        if (Bank is null) throw new DomainException("Bank info required");
        if (_serviceAreas.Count == 0) throw new DomainException("At least one ServiceArea required");
        if (_vehicles.Count == 0) throw new DomainException("At least one Vehicle required");
        if (!_rates.Any(r => r.IsActive)) throw new DomainException("At least one active Rate required");
        
        OnboardingStatus = OnboardingStatus.Submitted;
        // Public event: CarrierOnboardingSubmitted
    }
    
    public void Verify(Guid actorAdminUserId) { /* Public event: CarrierVerified — Identity carrier role grant */ }
    public void Suspend(string reason, Guid actorAdminUserId) { /* Public event: CarrierSuspended */ }
    public void Reactivate(Guid actorAdminUserId) { /* Public event: CarrierReactivated */ }
    
    // Fleet
    public Vehicle AddVehicle(VehicleType type, string plate, int capacityAnimals, decimal capacityWeightKg) { ... }
    public Driver AddDriver(string name, PhoneNumber phone, string licenseNumber) { ... }
    public void AssignDriverToVehicle(Guid driverId, Guid vehicleId) { ... }
    
    // Service Area / Zone / Rate
    public CarrierServiceArea AddServiceArea(string name, MultiPolygon area, CountryCode country) { ... }
    public CarrierZone AddZone(string name, CountryCode? country, IReadOnlyList<int>? regionLocationIds) { ... }
    public CarrierRate SetRate(Guid zoneId, LivestockKind kind, Money pricePerKm, Money? minCharge) { ... }
    
    // Auto-toggle
    public void ToggleAutoAssignment(bool enabled) { ... }
    
    public bool CanAcceptShipmentFor(LivestockKind kind, Guid pickupZoneId, Guid dropoffZoneId)
    {
        if (!AcceptsAutoAssignment) return false;
        if (OnboardingStatus != OnboardingStatus.Verified) return false;
        
        var pickupRate = _rates.FirstOrDefault(r => r.ZoneId == pickupZoneId && r.LivestockKind == kind);
        var dropoffRate = _rates.FirstOrDefault(r => r.ZoneId == dropoffZoneId && r.LivestockKind == kind);
        return pickupRate is not null && dropoffRate is not null;
    }
}
```

### `Shipment` AR

```csharp
public class Shipment
{
    public Guid Id;
    public Guid DealId;                            // ID-ref Marketplace
    public Guid CarrierId;                          // ID-ref Carrier
    public Guid SellerUserId, BuyerUserId;          // ID-ref Identity
    
    public GeoPoint PickupLocation;
    public string PickupAddress;
    public Guid? PickupFarmId;                       // Accounts.Farm opsiyonel
    public GeoPoint DropoffLocation;
    public string DropoffAddress;
    public Guid? DropoffFarmId;
    
    public LivestockKind Kind;
    public int AnimalCount;
    public decimal? EstimatedTotalWeightKg;
    
    public Money AgreedPrice;
    public DateTimeOffset ScheduledPickupAt;
    public DateTimeOffset? ActualPickupAt, DeliveredAt, CancelledAt;
    public string? CancelReason;
    
    public ShipmentStatus Status;
    public Guid? AssignedVehicleId, AssignedDriverId;
    
    public DateTimeOffset CreatedAt, UpdatedAt;
    
    public static Shipment Create(Guid dealId, Guid carrierId, Guid sellerUserId, Guid buyerUserId,
                                   ShipmentDetails details, Money agreedPrice)
    {
        // Marketplace → ICarrierShipmentCommands.CreateAsync ile çağrılır
        // Public event: CarrierShipmentCreated
    }
    
    public void AssignVehicleAndDriver(Guid vehicleId, Guid driverId) { /* Pending → Assigned */ }
    public void MarkPickedUp(DateTimeOffset pickedUpAt) { /* Assigned → InTransit, CarrierShipmentPickedUp */ }
    public void MarkDelivered(DateTimeOffset deliveredAt) { /* InTransit → Delivered, CarrierShipmentDelivered */ }
    public void Cancel(string reason, Guid actorUserId) { /* CarrierShipmentCancelled */ }
}

public enum ShipmentStatus
{
    Pending = 1, Assigned = 2, PickedUp = 3, InTransit = 4, Delivered = 5, Cancelled = 6
}
```

### `CarrierOffer` AR

```csharp
public class CarrierOffer
{
    public Guid Id;
    public Guid ShipmentRequestId;                  // Marketplace request reference
    public Guid DealId, CarrierId;
    public Money ProposedPrice;
    public DateTimeOffset ProposedPickupAt;
    public DateTimeOffset? EstimatedDeliveryAt;
    public string? Note;
    public CarrierOfferStatus Status;
    public DateTimeOffset CreatedAt, ExpiresAt;     // typical 24h TTL
    
    public static CarrierOffer Submit(Guid shipmentRequestId, Guid dealId, Guid carrierId,
                                       Money price, DateTimeOffset pickupAt, TimeSpan ttl) { ... }
    
    public void Accept(Guid actorUserId) { /* Pending → Accepted */ }
    public void Reject(Guid actorUserId) { ... }
    public void Withdraw() { ... }
}

public enum CarrierOfferStatus { Pending = 1, Accepted = 2, Rejected = 3, Withdrawn = 4, Expired = 5 }
```

---

## 3. Child Entities (Carrier AR İçinde)

### `CarrierServiceArea`

```csharp
public class CarrierServiceArea
{
    public Guid Id, CarrierId;
    public string Name;                              // "Marmara", "Ege"
    public MultiPolygon Area;                        // PostGIS MultiPolygon, SRID 4326
    public CountryCode Country;
    public bool IsActive;
}
```

### `CarrierZone`, `CarrierRate`

```csharp
public class CarrierZone
{
    public Guid Id, CarrierId;
    public string Name;                              // "Marmara Bölgesi"
    public CountryCode? Country;
    public IReadOnlyList<int> RegionLocationIds;     // Catalog Location ref'leri
    public bool IsActive;
}

public class CarrierRate
{
    public Guid Id, CarrierId, ZoneId;
    public LivestockKind LivestockKind;
    public Money PricePerKm;
    public Money? MinCharge;
    public bool IsActive;
}

public enum LivestockKind
{
    Cattle = 1, Sheep = 2, Goat = 3, Poultry = 4,
    Equine = 5, Beekeeping = 6, Mixed = 99
}
```

### `Vehicle`, `Driver` (Çekince 3 — VehicleType 7 değer)

```csharp
public class Vehicle
{
    public Guid Id, CarrierId;
    public string Plate;                             // "34 ABC 1234"
    public VehicleType Type;
    public int CapacityAnimals;
    public decimal CapacityWeightKg;
    public bool HasGps;                              // Faz 2 GPS tracking
    public bool IsActive;
    public DateTimeOffset? InsuranceExpiresAt;
}

// Çekince 3 — frontend hizalı 7 değer
public enum VehicleType
{
    StandardLivestockTruck = 1,     // frontend "standard"
    ClimateControlled = 2,           // frontend "climate"
    BreedingSpecial = 3,             // frontend "breeding_special"
    TirTrailer = 4,                  // frontend "tir_trailer"
    SmallAnimalVan = 5,
    BeehiveCarrier = 6,
    Refrigerated = 7
}

public class Driver
{
    public Guid Id, CarrierId;
    public string Name;
    public PhoneNumber Phone;
    public string LicenseNumber;                     // SRC belgesi no
    public DateTimeOffset? LicenseExpiresAt;
    public bool IsActive;
    
    // Faz 1: Carrier-internal employee (User reference YOK)
    // Faz 2: opsiyonel UserId field; driver mobile login
    public Guid? UserId;                              // Faz 2 schema-ready, Faz 1 null
}
```

---

## 4. Sync Command Pattern (Marketplace → Carrier)

### `ICarrierShipmentCommands` (Shared/)

```csharp
namespace Shared.Contracts.Carrier;

public interface ICarrierShipmentCommands
{
    // Marketplace (DealPaymentConfirmed) → Carrier shipment create
    Task<Result<Guid>> CreateAsync(
        Guid dealId, Guid carrierId, ShipmentDetails details, Money agreedPrice,
        CancellationToken ct);
    
    // Marketplace deal cancel → shipment cancel
    Task<Result> CancelByDealAsync(Guid dealId, string reason, CancellationToken ct);
    
    // Admin Carrier suspend → flag aktif shipment'lar (CarrierSuspended hybrid)
    Task<Result> FlagCarrierShipmentsAsync(Guid carrierId, CancellationToken ct);
}

public sealed record ShipmentDetails(
    Guid SellerUserId, Guid BuyerUserId,
    GeoPoint PickupLocation, string PickupAddress, Guid? PickupFarmId,
    GeoPoint DropoffLocation, string DropoffAddress, Guid? DropoffFarmId,
    LivestockKind Kind, int AnimalCount, decimal? EstimatedTotalWeightKg,
    DateTimeOffset ScheduledPickupAt);
```

Marketplace AgreementPaymentConfirmedHandler sync call eder; failure → Marketplace transaction rollback.

---

## 5. Cross-Modül Erişim

### `ICarrierReadService`

```csharp
public interface ICarrierReadService
{
    Task<CarrierSummary?> GetCarrierSummaryAsync(Guid carrierId, CancellationToken ct);
    Task<CarrierSummary?> GetCarrierSummaryByUserIdAsync(Guid userId, CancellationToken ct);
    Task<bool> IsActiveCarrierAsync(Guid userId, CancellationToken ct);
    Task<IReadOnlyList<CarrierMatchCandidate>> FindMatchingCarriersAsync(
        GeoPoint pickup, GeoPoint dropoff, LivestockKind kind, int animalCount, CancellationToken ct);
}
```

### `IAdminCarrierCommands` + `IAdminCarrierReadService`

Standart pattern (verify/reject/suspend/reactivate + queue list).

---

## 6. Public Event'ler (8)

| Event | Consumer |
|---|---|
| `CarrierOnboardingSubmitted` | Notifications, Admin queue |
| `CarrierVerified` | Identity (carrier role grant), Notifications |
| `CarrierSuspended` | Marketplace (sync FlagCarrierShipmentsAsync), Identity (role revoke), Subscription, Notifications |
| `CarrierReactivated` | Notifications |
| `CarrierShipmentCreated` | Marketplace (Deal status), Notifications |
| `CarrierShipmentPickedUp` | Marketplace (Deal InTransit), Notifications buyer |
| `CarrierShipmentDelivered` | Marketplace (Deal Delivered), Notifications buyer (confirm) |
| `CarrierShipmentCancelled` | Marketplace (Deal handling), Notifications |

**Internal:** CarrierZoneUpdated, CarrierRateUpdated, CarrierFleetVehicleAdded/Removed, CarrierDriverAssigned.

---

## 7. Auto-Toggle Rate Match Algoritması

```
Shipment Request (Marketplace → Carrier)
   ├─ Candidate carriers: AcceptsAutoAssignment=true && status=Verified
   ├─ Per carrier eligibility:
   │     1. ServiceArea pickup & dropoff PostGIS Covers check
   │     2. Pickup zone resolve (RegionLocationIds match)
   │     3. Dropoff zone resolve
   │     4. Rate var mı (zone × livestock kind)?
   │     5. Vehicle capacity yeterli mi?
   └─ Eligible carriers:
        - Otomatik CarrierOffer.Submit
        - veya frontend "Suggest carriers" listesi
```

---

## 8. API Endpoint Inventory (33)

### Authenticated — Carrier (12)

`POST/GET/PATCH /carrier/me` + `/business-info` + `/bank-info` + `/slug` + `/avatar` + `/cover-photo` + `/upload-url` + `/submit` + `/verification-status` + `/toggle-auto-assignment` + `/vehicles` (POST/PATCH/DELETE).

### Authenticated — Service Area / Zone / Rate (5)

`POST/PATCH/DELETE /carrier/me/service-areas` + `/zones` + `/rates` + `GET /rates-matrix` + `POST /drivers`.

### Authenticated — Shipment & Offers (8)

`GET /carrier/me/shipments?status=&cursor=` + `/{id}` + `POST /assign` + `/pickup` + `/deliver` + `GET /offers?status=` + `POST /offers` + `/{id}/withdraw`.

### Public (3)

`GET /carriers?cursor=&country=&serviceArea=` + `/{slugOrId}` + `/rates-matrix`.

### Admin (5)

`GET /admin/carrier/carriers?status=&cursor=` + `/{id}` + `POST verify/suspend/reactivate`.

---

## 9. Discovered Backlog

| # | Konu | Hedef |
|---|---|---|
| 97 | Auto-toggle algorithm inline mı event-driven mı | Karar 5 / Marketplace |
| 98 | Vehicle GPS tracking Faz 2 (SignalR vs MQTT) | Karar 7 / Realtime Faz 2 |
| 99 | Driver promote to User Faz 2 migration pattern | Karar 5 / Carrier Faz 2 |
| 100 | Insurance/license expiry alerts (cron, 30-day warning) | Karar 5 / Notifications |
| 101 | CarrierServiceArea polygon validation (ST_IsValid, self-intersection reddi) | Karar 5 / Carrier Faz 1 |
| 102 | Rate matrix sparse vs dense storage + index strategy | Karar 7 / Performance |
| 103 | VehicleType SmallAnimalVan/BeehiveCarrier/Refrigerated frontend görünüyor mu | Frontend ekibe görev |

---

## 10. Özet Tablo

| Konu | Karar |
|---|---|
| AR sayısı | 3 (Carrier, Shipment, CarrierOffer) |
| Child entities | ServiceArea, Zone, Rate, Vehicle, Driver, CarrierDocument |
| Public events | 8 |
| Cross-modül | `ICarrierShipmentCommands` (sync Marketplace tetikliyor), `ICarrierReadService` |
| PostGIS | ServiceArea MultiPolygon, GIST index `ix_carrier_service_areas_area_gist` |
| Auto-toggle | Rate-matrix-driven eligibility (zone × livestock kind) |
| VehicleType | 7 değer (frontend hizalı — Çekince 3) |
| Driver | Faz 1 internal employee; Faz 2 schema-ready UserId |
| Init pattern | InitDraft minimal (Çekince 4 — Seller D3 ile uyum) |
| Endpoint | 33 (12 Carrier + 5 ServiceArea/Zone/Rate + 8 Shipment/Offer + 3 Public + 5 Admin) |
