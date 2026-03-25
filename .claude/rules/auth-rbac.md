---
paths:
  - "**/Authorization/**"
  - "**/Auth/**"
  - "**/Verificator.cs"
---
# Authentication & JWT

- JWT-based with refresh tokens stored in `AppRefreshTokens` table
- Multi-provider: Native (email/password), Google OAuth, Apple Sign-In
- Roles stored per module in `UserRole` entity with `ModuleId`
- JWT role claims format: `"ModuleName.RoleName"` (e.g., `"LivestockTrading.Seller"`)
- Platform tracking: Web=0, Android=1, iOS=2

# Role-Based Authorization (RBAC)

Platform 5 temel rol ile calisir. Roller `Jobs/RelationalDB/MigrationJob/SeedData/roles.json` dosyasinda tanimlidir.

**Roller ve Yetkileri:**

| Rol | ID | Aciklama |
|-----|-----|----------|
| Admin | `a1000000-0000-0000-0000-000000000001` | Tam yetki, tum islemler |
| Moderator | `a1000000-0000-0000-0000-000000000002` | Icerik moderasyonu, onay/red |
| Seller | `a1000000-0000-0000-0000-000000000003` | Urun satisi, ciftlik yonetimi |
| Transporter | `a1000000-0000-0000-0000-000000000004` | Nakliye hizmetleri |
| Buyer | `a1000000-0000-0000-0000-000000000006` | Urun satin alma (varsayilan rol) |

**Otomatik Rol Atama:**
- Yeni kayit olan kullanicilara otomatik olarak `Buyer` rolu atanir
- `User/Commands/Create/Handler.cs` icinde `UserRole` kaydi olusturulur

## PermissionService Kullanimi

Verificator'larda rol kontrolu icin `PermissionService` kullanilir:

```csharp
// Application/Authorization/PermissionService.cs
using LivestockTrading.Application.Authorization;

public class Verificator : IRequestVerificator
{
    private readonly AuthorizationService _authorizationService;
    private readonly PermissionService _permissionService;

    public Verificator(ArfBlocksDependencyProvider dependencyProvider)
    {
        _authorizationService = dependencyProvider.GetInstance<AuthorizationService>();
        _permissionService = dependencyProvider.GetInstance<PermissionService>();
    }

    public async Task VerificateActor(IRequestModel payload, EndpointContext context, CancellationToken ct)
    {
        await _authorizationService
            .ForResource(typeof(Verificator).Namespace)
            .VerifyActor()
            .Assert();

        // Rol kontrolu - asagidaki methodlardan birini kullan
        _permissionService.RequireAdmin();           // Sadece Admin
        _permissionService.RequireModerator();       // Admin veya Moderator
        _permissionService.RequireSeller();          // Admin, Moderator veya Seller
        _permissionService.RequireTransporter();     // Admin, Moderator veya Transporter
        _permissionService.RequireAnyRole(RoleConstants.Seller, RoleConstants.Buyer);  // Belirtilen rollerden biri
    }
}
```

## Endpoint Rol Gereksinimleri

| Endpoint Tipi | Gerekli Rol | Method |
|---------------|-------------|--------|
| Products/Approve, Products/Reject | Admin, Moderator | `RequireModerator()` |
| Sellers/Verify, Sellers/Suspend | Admin, Moderator | `RequireModerator()` |
| Transporters/Verify, Transporters/Suspend | Admin, Moderator | `RequireModerator()` |
| Brands, Banners, Currencies, FAQs CRUD | Admin, Moderator | `RequireModerator()` |
| Products/Create, Products/Update | Seller | `RequireSeller()` |
| Farms, ProductImages, ProductPrices CRUD | Seller (kendi kaynaklari) | `RequireSeller()` |
| TransportRequests CRUD | Transporter | `RequireTransporter()` |
| FavoriteProducts, ProductReviews CRUD | Buyer | `RequireAnyRole(Buyer, Seller)` |

## Yeni Moderasyon Endpoint'leri

**Products:**
- `POST /Products/Approve` - Urunu onayla (Status -> Approved)
- `POST /Products/Reject` - Urunu reddet (Status -> Rejected, reason kaydedilir)

**Sellers:**
- `POST /Sellers/Verify` - Saticiyi dogrula (Status -> Verified)
- `POST /Sellers/Suspend` - Saticiyi askiya al (Status -> Suspended, reason kaydedilir)

**Transporters:**
- `POST /Transporters/Verify` - Tasiyiciyi dogrula (Status -> Verified)
- `POST /Transporters/Suspend` - Tasiyiciyi askiya al (Status -> Suspended, reason kaydedilir)

## DI Kaydi
```csharp
// ApplicationDependencyProvider.cs
base.Add<PermissionService>();
```

## Ilgili Dosyalar

- `Application/Authorization/PermissionService.cs` - Rol kontrol servisi
- `Application/Authorization/RoleConstants.cs` - Rol sabitleri
- `Application/Authorization/RequirePermissionAttribute.cs` - Attribute (opsiyonel)
- `Domain/Enums/Permission.cs` - Permission enum'lari
- `Jobs/RelationalDB/MigrationJob/SeedData/roles.json` - Rol seed data
