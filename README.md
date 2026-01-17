# MADEN BACKEND - KOD STANDARTLARI

## 📁 DOSYA & KLASÖR İSİMLENDİRME

### RequestHandlers Yapısı:
```
RequestHandlers/
└── [EntityName]/                    # PascalCase, çoğul (Animals, Bids, Farms)
    ├── Commands/                    # Yazma işlemleri
    │   ├── Create/
    │   ├── Update/
    │   ├── Delete/
    │   └── [CustomAction]/          # Accept, Reject, Cancel vb.
    └── Queries/                     # Okuma işlemleri
        ├── All/                     # Liste (filtreleme, sıralama, sayfalama)
        ├── Pick/                    # Tekil kayıt (ID ile)
        └── [CustomQuery]/           # GetByUser, GetActive vb.
```

### Yeni Entity Handler Yapıları:
```
RequestHandlers/
├── EarTags/                         # Küpe takibi
│   ├── Commands/
│   │   ├── Create/                  # Yeni küpe kaydı
│   │   ├── Update/                  # Küpe bilgisi güncelleme
│   │   └── Delete/                  # Küpe silme (soft delete)
│   └── Queries/
│       ├── All/                     # Küpe listesi
│       ├── Pick/                    # Tekil küpe
│       └── GetByAnimal/             # Hayvana ait küpeler
│
└── ShipmentRecords/                 # Sevk kayıtları
    ├── Commands/
    │   ├── Create/                  # Yeni sevk kaydı
    │   ├── Update/                  # Sevk güncelleme
    │   ├── MarkDelivered/           # Teslim edildi işaretle
    │   └── Cancel/                  # Sevk iptal
    └── Queries/
        ├── All/                     # Sevk listesi
        ├── Pick/                    # Tekil sevk kaydı
        └── GetByDeliveryOrder/      # Teslimat siparişine göre
```

### Handler Dosyaları:
| Dosya | Zorunlu | Açıklama |
|-------|---------|----------|
| `Handler.cs` |  EVET | İş mantığı |
| `DataAccess.cs` |  EVET | Veritabanı işlemleri |
| `Validator.cs` |  EVET | Input validasyonu |
| `Models.cs` |  EVET | Request/Response modelleri |
| `Mapper.cs` |  EVET | Entity ↔ DTO dönüşümü |
| `Verificator.cs` | ❌ HAYIR | Authorization kontrolü (opsiyonel) |

---

## ⚠️ KRİTİK KURAL: IDataAccess

### IDataAccess Interface'i Manuel Güncellenmez!

DataAccess sınıfı `IDataAccess`'i **implement eder** ama interface'in kendisi **asla değiştirilmez**.
```csharp
//  DOĞRU - IDataAccess implement edilir
public class DataAccess : IDataAccess
{
    private readonly AnimalMarketModuleDbContext _dbContext;

    public DataAccess(ArfBlocksDependencyProvider dp)
    {
        _dbContext = dp.GetInstance<AnimalMarketModuleDbContext>();
    }

    // Kendi metodlarını tanımla
    public async Task<Animal> GetById(Guid id, CancellationToken ct)
    {
        return await _dbContext.Animals
            .FirstOrDefaultAsync(a => a.Id == id && !a.IsDeleted, ct);
    }
}
```
```csharp
// ❌ YANLIŞ - IDataAccess interface'ini değiştirme!
public interface IDataAccess
{
    // BURAYA DOKUNMA!
    // Bu interface framework tarafından yönetilir
}
```

### Özet:
| Ne? | Yapılabilir mi? |
|-----|-----------------|
| `DataAccess : IDataAccess` implement etmek |  EVET |
| `IDataAccess` interface'ine metod eklemek | ❌ HAYIR |
| `IDataAccess` interface'ini değiştirmek | ❌ HAYIR |
| DataAccess içine kendi metodlarını yazmak |  EVET |

---

## 🏗️ HANDLER YAPISI

### Standart Handler Şablonu:
```csharp
namespace BusinessModules.AnimalMarket.Application.RequestHandlers.Animals.Commands.Create;

public class Handler : IRequestHandler
{
    // 1. Private fields - readonly
    private readonly DataAccess _da;
    private readonly CurrentUserService _currentUserService;
    private readonly BidPlacedNotificationPublisher _publisher;

    // 2. Constructor - ArfBlocksDependencyProvider ile
    public Handler(ArfBlocksDependencyProvider dependencyProvider, object dataAccess)
    {
        _da = (DataAccess)dataAccess;
        _currentUserService = dependencyProvider.GetInstance<CurrentUserService>();
        _publisher = dependencyProvider.GetInstance<BidPlacedNotificationPublisher>();
    }

    // 3. Handle metodu
    public async Task<ArfBlocksRequestResult> Handle(
        IRequestModel payload, 
        EndpointContext context, 
        CancellationToken ct)
    {
        var request = (RequestModel)payload;
        var mapper = new Mapper();

        // İş mantığı...
        var entity = mapper.MapToEntity(request);
        await _da.Create(entity, ct);

        // Response
        var response = mapper.MapToResponse(entity);
        return ArfBlocksResults.Success(response);
    }
}
```

---

## 📊 DATAACCESS YAPISI

### Standart DataAccess Şablonu:
```csharp
namespace BusinessModules.AnimalMarket.Application.RequestHandlers.Animals.Commands.Create;

public class DataAccess : IDataAccess
{
    private readonly AnimalMarketModuleDbContext _dbContext;

    public DataAccess(ArfBlocksDependencyProvider dp)
    {
        _dbContext = dp.GetInstance<AnimalMarketModuleDbContext>();
    }

    // Tek kayıt getirme
    public async Task<Animal> GetById(Guid id, CancellationToken ct)
    {
        return await _dbContext.Animals
            .Include(a => a.Farm)
            .Include(a => a.Breed)
            .FirstOrDefaultAsync(a => a.Id == id && !a.IsDeleted, ct);
    }

    // Liste getirme
    public async Task<List<Animal>> GetByFarmId(Guid farmId, CancellationToken ct)
    {
        return await _dbContext.Animals
            .Where(a => a.FarmId == farmId && !a.IsDeleted)
            .OrderByDescending(a => a.CreatedAt)
            .ToListAsync(ct);
    }

    // Oluşturma
    public async Task Create(Animal entity, CancellationToken ct)
    {
        await _dbContext.Animals.AddAsync(entity, ct);
        await _dbContext.SaveChangesAsync(ct);
    }

    // Güncelleme
    public async Task Update(Animal entity, CancellationToken ct)
    {
        _dbContext.Animals.Update(entity);
        await _dbContext.SaveChangesAsync(ct);
    }

    // Soft delete
    public async Task Delete(Animal entity, CancellationToken ct)
    {
        entity.IsDeleted = true;
        entity.UpdatedAt = DateTime.UtcNow;
        await _dbContext.SaveChangesAsync(ct);
    }
}
```

### Include Kuralları:
```csharp
//  DOĞRU - Sadece ihtiyaç duyulan navigation'lar
return await _dbContext.Animals
    .Include(a => a.Farm)
    .Include(a => a.Breed)
    .FirstOrDefaultAsync(a => a.Id == id, ct);

// ❌ YANLIŞ - Gereksiz include spam
return await _dbContext.Animals
    .Include(a => a.Farm)
        .ThenInclude(f => f.Owner)
            .ThenInclude(o => o.Company)
    .Include(a => a.Breed)
    .Include(a => a.Photos)
    .Include(a => a.Bids)
    .Include(a => a.Documents)
    .FirstOrDefaultAsync(a => a.Id == id, ct);
```

---

##  VALIDATOR YAPISI

### Standart Validator Şablonu:
```csharp
namespace BusinessModules.AnimalMarket.Application.RequestHandlers.Animals.Commands.Create;

public class Validator : IRequestValidator
{
    private readonly AnimalMarketModuleDbValidationService _validation;

    public Validator(ArfBlocksDependencyProvider dp)
    {
        _validation = dp.GetInstance<AnimalMarketModuleDbValidationService>();
    }

    // 1. Request model validasyonu (senkron)
    public void ValidateRequestModel(
        IRequestModel payload, 
        EndpointContext context, 
        CancellationToken ct)
    {
        var m = (RequestModel)payload;
        var result = new RequestModel_Validator().Validate(m);
        
        if (!result.IsValid)
            throw new ArfBlocksValidationException(result.ToString("~"));
    }

    // 2. Domain validasyonu (async - DB kontrolleri)
    public async Task ValidateDomain(
        IRequestModel payload, 
        EndpointContext context, 
        CancellationToken ct)
    {
        var m = (RequestModel)payload;
        
        await _validation.ValidateFarmExist(m.FarmId);
        await _validation.ValidateBreedExist(m.BreedId);
        await _validation.ValidateUserExist(m.SellerId);
    }
}
```

### FluentValidation Kuralları:
```csharp
public class RequestModel_Validator : AbstractValidator<RequestModel>
{
    public RequestModel_Validator()
    {
        // Required alanlar
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage(ErrorCodeGenerator.GetErrorCode(
                () => AnimalMarketDomainErrors.AnimalMarketAnimalErrors.NameRequired));

        // String uzunluk
        RuleFor(x => x.Name)
            .MaximumLength(200)
            .WithMessage(ErrorCodeGenerator.GetErrorCode(
                () => AnimalMarketDomainErrors.CommonErrors.ValueTooLong));

        // Sayısal değer
        RuleFor(x => x.Price)
            .GreaterThan(0)
            .WithMessage(ErrorCodeGenerator.GetErrorCode(
                () => AnimalMarketDomainErrors.AnimalMarketAnimalErrors.InvalidPrice));

        // Guid validasyonu
        RuleFor(x => x.FarmId)
            .NotEmpty()
            .WithMessage(ErrorCodeGenerator.GetErrorCode(
                () => AnimalMarketDomainErrors.AnimalMarketFarmErrors.FarmIdRequired));

        // Enum validasyonu
        RuleFor(x => x.Status)
            .IsInEnum()
            .WithMessage(ErrorCodeGenerator.GetErrorCode(
                () => AnimalMarketDomainErrors.CommonErrors.InvalidEnum));

        // Conditional validation
        When(x => x.SaleType == AnimalSaleType.Auction, () =>
        {
            RuleFor(x => x.AuctionEndDate)
                .NotEmpty()
                .GreaterThan(DateTime.UtcNow)
                .WithMessage(ErrorCodeGenerator.GetErrorCode(
                    () => AnimalMarketDomainErrors.AnimalMarketAuctionErrors.InvalidEndDate));
        });
    }
}
```

---

## 🔐 VERIFICATOR YAPISI

### Standart Verificator Şablonu:
```csharp
namespace BusinessModules.AnimalMarket.Application.RequestHandlers.Animals.Commands.Update;

public class Verificator : IRequestVerificator
{
    private readonly AuthorizationService _auth;
    private readonly AnimalMarketModuleDbVerificationService _verify;
    private readonly CurrentUserService _current;

    public Verificator(ArfBlocksDependencyProvider dp)
    {
        _auth = dp.GetInstance<AuthorizationService>();
        _verify = dp.GetInstance<AnimalMarketModuleDbVerificationService>();
        _current = dp.GetInstance<CurrentUserService>();
    }

    // 1. Actor doğrulama (rol kontrolü)
    public async Task VerificateActor(
        IRequestModel payload, 
        EndpointContext ctx, 
        CancellationToken ct)
    {
        await _auth
            .ForResource(typeof(Verificator).Namespace)
            .VerifyActor()
            .Assert();
    }

    // 2. Domain doğrulama (yetki kontrolü)
    public async Task VerificateDomain(
        IRequestModel payload, 
        EndpointContext ctx, 
        CancellationToken ct)
    {
        var req = (RequestModel)payload;
        var currentUserId = _current.GetCurrentUserId();

        // Kullanıcı kendi kaydını mı güncelliyor?
        await _verify.ValidateAnimalOwnership(req.AnimalId, currentUserId);
    }
}
```

### Verificator vs Validator:
| Özellik | Validator | Verificator |
|---------|-----------|-------------|
| Amaç | Input validasyonu | Authorization kontrolü |
| Zorunlu |  EVET | ❌ HAYIR |
| İçerik | FluentValidation | Rol/yetki kontrolleri |
| Çalışma | Önce çalışır | Validator'dan sonra |
| Hata | ValidationException | UnauthorizedException |

---

## 📦 MODELS YAPISI

### Request Model:
```csharp
namespace BusinessModules.AnimalMarket.Application.RequestHandlers.Animals.Commands.Create;

public class RequestModel : IRequestModel
{
    public string Name { get; set; }
    public string Description { get; set; }
    public decimal Price { get; set; }
    public Guid FarmId { get; set; }
    public Guid BreedId { get; set; }
    public AnimalType Type { get; set; }
    public AnimalGender Gender { get; set; }
    public AnimalSaleType SaleType { get; set; }
    public DateTime? AuctionEndDate { get; set; }
}
```

### Response Model:
```csharp
public class ResponseModel
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public decimal Price { get; set; }
    public AnimalStatus Status { get; set; }
    public DateTime CreatedAt { get; set; }
    
    // Navigation property'ler için nested class
    public FarmDto Farm { get; set; }
    public BreedDto Breed { get; set; }
}

public class FarmDto
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string City { get; set; }
}

public class BreedDto
{
    public Guid Id { get; set; }
    public string Name { get; set; }
}
```

---

## 🔄 MAPPER YAPISI

### Standart Mapper Şablonu:
```csharp
namespace BusinessModules.AnimalMarket.Application.RequestHandlers.Animals.Commands.Create;

public class Mapper
{
    // Request → Entity
    public Animal MapToEntity(RequestModel request)
    {
        return new Animal
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            Description = request.Description,
            Price = request.Price,
            FarmId = request.FarmId,
            BreedId = request.BreedId,
            Type = request.Type,
            Gender = request.Gender,
            SaleType = request.SaleType,
            AuctionEndDate = request.AuctionEndDate,
            Status = AnimalStatus.PendingApproval,
            CreatedAt = DateTime.UtcNow,
            IsDeleted = false
        };
    }

    // Entity → Response
    public ResponseModel MapToResponse(Animal entity)
    {
        return new ResponseModel
        {
            Id = entity.Id,
            Name = entity.Name,
            Description = entity.Description,
            Price = entity.Price,
            Status = entity.Status,
            CreatedAt = entity.CreatedAt,
            Farm = entity.Farm != null ? new FarmDto
            {
                Id = entity.Farm.Id,
                Name = entity.Farm.Name,
                City = entity.Farm.City
            } : null,
            Breed = entity.Breed != null ? new BreedDto
            {
                Id = entity.Breed.Id,
                Name = entity.Breed.Name
            } : null
        };
    }

    // Update mapping
    public void MapToUpdate(Animal entity, RequestModel request)
    {
        entity.Name = request.Name;
        entity.Description = request.Description;
        entity.Price = request.Price;
        entity.UpdatedAt = DateTime.UtcNow;
    }
}
```

---

## 🗃️ ENTITY TANIMLAMA

### Standart Entity Şablonu:
```csharp
namespace Common.Definitions.Domain.AnimalMarket.Entities;

public class Animal : BaseEntity, ITenantEntity
{
    // Primary fields
    public string Name { get; set; }
    public string Description { get; set; }
    public decimal Price { get; set; }
    public decimal? ReservePrice { get; set; }

    // Enums
    public AnimalType Type { get; set; }
    public AnimalGender Gender { get; set; }
    public AnimalStatus Status { get; set; }
    public AnimalSaleType SaleType { get; set; }

    // Flags
    public bool IsForSacrifice { get; set; }  // Kurbanlık mı?

    // Foreign keys
    public Guid FarmId { get; set; }
    public Guid BreedId { get; set; }
    public Guid SellerId { get; set; }

    // Dates
    public DateTime? AuctionEndDate { get; set; }
    public DateTime? SoldAt { get; set; }

    // Navigation properties
    public Farm Farm { get; set; }
    public Breed Breed { get; set; }
    public User Seller { get; set; }
    public ICollection<Bid> Bids { get; set; }
    public ICollection<AnimalPhoto> Photos { get; set; }
    public ICollection<AnimalEarTag> EarTags { get; set; }  // Küpe kayıtları

    // ITenantEntity implementation
    public Guid GetTenantId() => this.FarmId;
    public string GetTenantPropertyName() => "FarmId";
    public object GetTenantEntity() => this.Farm;
}
```

### BaseEntity İçeriği:
```csharp
public abstract class BaseEntity
{
    public Guid Id { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public bool IsDeleted { get; set; }
}
```

### AnimalEarTag Entity (Küpe Takibi):
```csharp
namespace BusinessModules.AnimalMarket.Domain.Entities;

public class AnimalEarTag : BaseEntity
{
    // Küpe numarası - TR + 12 haneli sayı formatı (benzersiz)
    public string EarTagNumber { get; set; }

    // Foreign keys
    public Guid AnimalId { get; set; }
    public Guid? BreedId { get; set; }

    // Hayvan bilgileri
    public AnimalType Type { get; set; }
    public AnimalGender Gender { get; set; }
    public DateTime? BirthDate { get; set; }
    public decimal? Weight { get; set; }
    public bool IsPregnant { get; set; }
    public string HealthNotes { get; set; }

    // Kayıt bilgileri
    public DateTime RegisteredAt { get; set; }
    public EarTagStatus Status { get; set; }

    // Navigation properties
    public Animal Animal { get; set; }
    public AnimalBreed Breed { get; set; }
    public ICollection<ShipmentRecord> ShipmentRecords { get; set; }
}

public enum EarTagStatus
{
    Active = 0,      // Aktif küpe
    Shipped = 1,     // Sevk edildi
    Delivered = 2,   // Teslim edildi
    Cancelled = 3    // İptal edildi
}
```

### ShipmentRecord Entity (Sevk Kaydı):
```csharp
namespace BusinessModules.AnimalMarket.Domain.Entities;

public class ShipmentRecord : BaseEntity
{
    // Foreign keys
    public Guid DeliveryOrderId { get; set; }
    public Guid AnimalId { get; set; }
    public Guid EarTagId { get; set; }

    // Sevk bilgileri
    public DateTime ShipmentDate { get; set; }
    public DateTime? DeliveryDate { get; set; }
    public decimal? ShipmentWeight { get; set; }
    public decimal? DeliveryWeight { get; set; }

    // Sağlık durumu
    public string HealthStatusAtShipment { get; set; }
    public string HealthStatusAtDelivery { get; set; }

    // Durum ve onay
    public ShipmentStatus Status { get; set; }
    public bool VetApproved { get; set; }
    public DateTime? VetApprovedAt { get; set; }
    public Guid? ApprovedByVetId { get; set; }

    // Belgeler
    public string ShipmentDocumentUrl { get; set; }
    public string Notes { get; set; }

    // Navigation properties
    public DeliveryOrder DeliveryOrder { get; set; }
    public Animal Animal { get; set; }
    public AnimalEarTag EarTag { get; set; }
}

public enum ShipmentStatus
{
    Preparing = 0,   // Hazırlanıyor
    InTransit = 1,   // Yolda
    Delivered = 2,   // Teslim edildi
    Cancelled = 3,   // İptal edildi
    Issue = 4        // Sorun var
}
```

---

## 🚨 HATA YÖNETİMİ

### DomainErrors Tanımlama:
```csharp
// Common/Definitions/Domain/AnimalMarket/DomainErrors.cs

public static class AnimalMarketDomainErrors
{
    public class AnimalMarketAnimalErrors
    {
        public static string AnimalNotFound { get; set; } = "Hayvan bulunamadı.";
        public static string AnimalAlreadySold { get; set; } = "Hayvan zaten satılmış.";
        public static string NameRequired { get; set; } = "Hayvan adı gereklidir.";
        public static string InvalidPrice { get; set; } = "Geçersiz fiyat.";
        public static string CannotBidOwnAnimal { get; set; } = "Kendi hayvanınıza teklif veremezsiniz.";
    }

    public class AnimalMarketBidErrors
    {
        public static string BidNotFound { get; set; } = "Teklif bulunamadı.";
        public static string BidAlreadyAccepted { get; set; } = "Teklif zaten kabul edilmiş.";
        public static string AmountMustBeGreaterThanZero { get; set; } = "Teklif tutarı sıfırdan büyük olmalıdır.";
        public static string AuctionEnded { get; set; } = "Açık artırma sona ermiş.";
    }

    public class AnimalMarketEarTagErrors
    {
        public static string EarTagNotFound { get; set; } = "Küpe kaydı bulunamadı.";
        public static string EarTagNumberRequired { get; set; } = "Küpe numarası gereklidir.";
        public static string EarTagNumberInvalid { get; set; } = "Küpe numarası geçersiz formatda.";
        public static string EarTagNumberAlreadyExists { get; set; } = "Bu küpe numarası zaten kayıtlı.";
        public static string EarTagAlreadyShipped { get; set; } = "Küpe zaten sevk edilmiş.";
        public static string AnimalIdRequired { get; set; } = "Hayvan ID gereklidir.";
    }

    public class AnimalMarketShipmentRecordErrors
    {
        public static string ShipmentRecordNotFound { get; set; } = "Sevk kaydı bulunamadı.";
        public static string DeliveryOrderIdRequired { get; set; } = "Teslimat siparişi ID gereklidir.";
        public static string EarTagIdRequired { get; set; } = "Küpe ID gereklidir.";
        public static string ShipmentDateRequired { get; set; } = "Sevk tarihi gereklidir.";
        public static string ShipmentAlreadyDelivered { get; set; } = "Sevk zaten teslim edilmiş.";
        public static string DuplicateShipmentRecord { get; set; } = "Bu teslimat için bu küpe zaten kayıtlı.";
    }

    public class CommonErrors
    {
        public static string IdNotValid { get; set; } = "ID geçersiz.";
        public static string UnauthorizedAccess { get; set; } = "Yetkisiz erişim.";
        public static string InvalidEnum { get; set; } = "Enum değeri geçersiz.";
        public static string ValueTooLong { get; set; } = "Değer çok uzun.";
    }
}
```

### Hata Fırlatma:
```csharp
// Entity bulunamadı
if (animal == null)
{
    throw new ArfBlocksValidationException(
        ErrorCodeGenerator.GetErrorCode(
            () => AnimalMarketDomainErrors.AnimalMarketAnimalErrors.AnimalNotFound));
}

// İş kuralı ihlali
if (animal.SellerId == currentUserId)
{
    throw new ArfBlocksValidationException(
        ErrorCodeGenerator.GetErrorCode(
            () => AnimalMarketDomainErrors.AnimalMarketAnimalErrors.CannotBidOwnAnimal));
}

// Durum kontrolü
if (animal.Status == AnimalStatus.Sold)
{
    throw new ArfBlocksValidationException(
        ErrorCodeGenerator.GetErrorCode(
            () => AnimalMarketDomainErrors.AnimalMarketAnimalErrors.AnimalAlreadySold));
}
```

---

## 📨 EVENT PUBLISHING

### Publisher Tanımlama:
```csharp
namespace BusinessModules.AnimalMarket.Application.Publishers;

public class BidPlacedNotificationPublisher
{
    private readonly IRabbitMqPublisher _publisher;

    public BidPlacedNotificationPublisher(IRabbitMqPublisher publisher)
    {
        _publisher = publisher;
    }

    public async Task PublishAsync(BidPlacedEvent eventData)
    {
        await _publisher.PublishFanout("animalmarket.notification.push", eventData);
    }
}
```

### Event Tanımlama:
```csharp
public class BidPlacedEvent
{
    public Guid AnimalId { get; set; }
    public Guid BidderId { get; set; }
    public Guid SellerId { get; set; }
    public decimal BidAmount { get; set; }
    public string AnimalName { get; set; }
    public string BidderName { get; set; }
    public DateTime PlacedAt { get; set; }
}
```

### Handler'da Kullanım:
```csharp
public async Task<ArfBlocksRequestResult> Handle(...)
{
    // ... bid oluşturma işlemi

    // Event publish
    var eventData = new BidPlacedEvent
    {
        AnimalId = bid.AnimalId,
        BidderId = bid.BidderId,
        SellerId = animal.SellerId,
        BidAmount = bid.Amount,
        AnimalName = animal.Name,
        PlacedAt = DateTime.UtcNow
    };

    await _publisher.PublishAsync(eventData);

    return ArfBlocksResults.Success(response);
}
```

### Publisher DI Kaydı:
```csharp
// ApplicationDependencyProvider.cs
public override void AddDomainServices()
{
    base.Add<BidPlacedNotificationPublisher>();
    base.Add<PaymentCompletedEmailPublisher>();
    base.Add<DeliveryCreatedNotificationPublisher>();
}
```

---

## 🔤 İSİMLENDİRME KURALLARI

### Genel Kurallar:
| Tip | Format | Örnek |
|-----|--------|-------|
| Class | PascalCase | `AnimalService`, `BidValidator` |
| Interface | I + PascalCase | `IRequestHandler`, `ITenantEntity` |
| Method | PascalCase | `GetById`, `ValidateDomain` |
| Property | PascalCase | `AnimalId`, `CreatedAt` |
| Private field | _ + camelCase | `_dbContext`, `_currentUserService` |
| Parameter | camelCase | `animalId`, `cancellationToken` |
| Local variable | camelCase | `animal`, `currentUser` |
| Constant | UPPER_SNAKE | `MAX_BID_COUNT`, `DEFAULT_PAGE_SIZE` |

### Async Metod İsimlendirme:
```csharp
//  DOĞRU - Async suffix
public async Task<Animal> GetByIdAsync(Guid id, CancellationToken ct)
public async Task CreateAsync(Animal entity, CancellationToken ct)

//  DOĞRU - Handler'da suffix yok (convention)
public async Task<ArfBlocksRequestResult> Handle(...)

// ❌ YANLIŞ
public async Task<Animal> GetById(Guid id)  // Async suffix yok
public Animal GetByIdAsync(Guid id)         // async yok ama suffix var
```

### CancellationToken Kısaltması:
```csharp
//  DOĞRU - ct kısaltması
public async Task<Animal> GetById(Guid id, CancellationToken ct)

// ❌ YANLIŞ
public async Task<Animal> GetById(Guid id, CancellationToken cancellationToken)
public async Task<Animal> GetById(Guid id, CancellationToken token)
```

---

## 📝 YORUM KURALLARI

###  DOĞRU - İş Kuralı Açıklaması:
```csharp
// Kullanıcı kendi ilanına teklif veremez
if (animal.SellerId == currentUserId)
{
    throw new ArfBlocksValidationException(...);
}

// Açık artırma süresi dolmuşsa yeni teklif kabul edilmez
if (animal.AuctionEndDate < DateTime.UtcNow)
{
    throw new ArfBlocksValidationException(...);
}

// Veteriner onayı olmadan para satıcıya aktarılamaz
if (escrow.Status != EscrowStatus.Funded || !vetApproval.IsApproved)
{
    throw new ArfBlocksValidationException(...);
}
```

###  DOĞRU - Adım Numaralandırma:
```csharp
public async Task<ArfBlocksRequestResult> Handle(...)
{
    var request = (RequestModel)payload;
    var mapper = new Mapper();

    //  1. Hayvanı getir
    var animal = await _da.GetById(request.AnimalId, ct);

    //  2. Mevcut teklifi kontrol et
    var existingBid = await _da.GetActiveBidByUser(request.AnimalId, currentUserId, ct);

    //  3. Yeni teklif oluştur
    var bid = mapper.MapToEntity(request, currentUserId);
    await _da.Create(bid, ct);

    //  4. Bildirim gönder
    await _publisher.PublishAsync(new BidPlacedEvent { ... });

    //  5. Response döndür
    return ArfBlocksResults.Success(mapper.MapToResponse(bid));
}
```

### ❌ YANLIŞ - Gereksiz Yorumlar:
```csharp
// DataAccess'i al
_da = (DataAccess)dataAccess;

// Entity'yi kaydet
await _da.Create(entity, ct);

// Response döndür
return ArfBlocksResults.Success(response);
```

---

##  CHECKLIST - Her Handler İçin
```
□ Handler.cs - İş mantığı doğru mu?
□ DataAccess.cs - IDataAccess implement edildi mi? (interface değiştirilmedi mi?)
□ Validator.cs - FluentValidation kuralları tam mı?
□ Models.cs - Request/Response tanımlı mı?
□ Mapper.cs - Entity ↔ DTO dönüşümü var mı?
□ Verificator.cs - Yetki kontrolü gerekli mi?
□ DomainErrors - Hata mesajları tanımlı mı?
□ Migration - Yeni entity/kolon için migration var mı?
□ Publisher - Event publish gerekli mi?
□ SaveChangesAsync - Veritabanı kaydedildi mi?
```

---

## 🚫 YAPILMAMASI GEREKENLER

| ❌ YAPMA |  YAP |
|----------|--------|
| `IDataAccess` interface'ini değiştirme | `DataAccess : IDataAccess` implement et |
| Senkron DB çağrısı | `async/await` kullan |
| `try-catch` ile hata yutma | `ArfBlocksValidationException` fırlat |
| Hardcoded connection string | Environment variable kullan |
| Handler'da direkt DbContext | DataAccess üzerinden git |
| `SaveChanges()` unutma | Her işlem sonunda `SaveChangesAsync()` |
| Magic string hata mesajı | `ErrorCodeGenerator` kullan |
| Gereksiz Include | Sadece ihtiyaç duyulanları Include et |

---

## 📋 MİGRATION KURALLARI

### Yeni Migration Oluşturma:
```bash
cd Jobs/RelationalDB/MigrationJob
dotnet ef migrations add MigrationAdi
```

### Migration Çalıştırma:
```bash
dotnet run development   # Dev ortamı
dotnet run production    # Prod ortamı
```

### Migration İsimlendirme:
```
 DOĞRU:
- AddAnimalReservePrice
- AddPaymentTransactionTable
- UpdateBidStatusColumn
- AddIndexToAnimalFarmId

❌ YANLIŞ:
- Migration1
- Update
- Fix
- NewChanges
```

### Son Migration'lar:
| Migration | Tarih | Açıklama |
|-----------|-------|----------|
| `InitialCreate` | 2024-12-24 | Başlangıç şeması |
| `AddVeterinarianProfilePersonalFields` | 2024-12-25 | VeterinarianProfile'a NationalIdNumber eklendi |
| `AddIsForSacrificeToAnimal` | 2024-12-26 | Animal'a IsForSacrifice (kurbanlık) flag eklendi |
| `AddAnimalEarTagAndShipmentRecord` | 2024-12-26 | Küpe takibi ve sevk kayıtları tabloları eklendi |

---

## 🗄️ VERİTABANI ŞEMASI

### Tablolar ve İlişkiler:
```
AnimalMarket_Animals
├── AnimalMarket_AnimalEarTags (1:M) - Küpe kayıtları
├── AnimalMarket_Bids (1:M)
├── AnimalMarket_AnimalPhotos (1:M)
└── AnimalMarket_Farms (M:1)

AnimalMarket_AnimalEarTags
├── AnimalMarket_Animals (M:1)
├── AnimalMarket_AnimalBreeds (M:1)
└── AnimalMarket_ShipmentRecords (1:M)

AnimalMarket_ShipmentRecords
├── AnimalMarket_DeliveryOrders (M:1)
├── AnimalMarket_Animals (M:1)
└── AnimalMarket_AnimalEarTags (M:1)
```

### Unique Constraints:
| Tablo | Constraint | Açıklama |
|-------|------------|----------|
| `AnimalEarTags` | `EarTagNumber` | Küpe numarası benzersiz (TR + 12 hane) |
| `ShipmentRecords` | `DeliveryOrderId + EarTagId` | Aynı teslimat için aynı küpe tek kez |

### Önemli Index'ler:
```
AnimalMarket_AnimalEarTags:
- IX_EarTagNumber (UNIQUE)
- IX_AnimalId_Status
- IX_BreedId
- IX_Type

AnimalMarket_ShipmentRecords:
- IX_DeliveryOrderId_EarTagId (UNIQUE)
- IX_AnimalId
- IX_EarTagId
- IX_DeliveryOrderId_Status
- IX_ShipmentDate
```