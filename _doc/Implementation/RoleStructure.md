# Rol Yapısı Implementasyon Planı

Bu döküman, GlobalLivestock platformu için rol tabanlı yetkilendirme sisteminin implementasyonunu detaylı olarak açıklar.

## 1. Rol Haritası

```
                                    ┌─────────────────┐
                                    │      ADMIN      │
                                    │  (Tam Yetki)    │
                                    └────────┬────────┘
                                             │
                    ┌────────────────────────┼────────────────────────┐
                    │                        │                        │
           ┌────────▼────────┐     ┌────────▼────────┐     ┌────────▼────────┐
           │    MODERATOR    │     │     SUPPORT     │     │   VETERINARIAN  │
           │ (İçerik Yönet.) │     │ (Müşteri Dest.) │     │  (Sağlık Onay)  │
           └────────┬────────┘     └─────────────────┘     └─────────────────┘
                    │
    ┌───────────────┴───────────────┐
    │                               │
┌───▼───────────┐         ┌────────▼────────┐
│    SELLER     │         │   TRANSPORTER   │
│   (Satıcı)    │◄───────►│   (Nakliyeci)   │
└───────┬───────┘         └────────┬────────┘
        │                          │
        │         ┌────────────────┘
        │         │
        ▼         ▼
    ┌─────────────────┐
    │      BUYER      │
    │ (Alıcı/Varsayılan) │
    └─────────────────┘
```

---

## 2. Rol Tanımları

| Rol | Kod | Açıklama | Varsayılan |
|-----|-----|----------|------------|
| Admin | `admin` | Platform yöneticisi, tam yetki | Hayır |
| Moderator | `moderator` | İçerik moderatörü, ürün onaylama | Hayır |
| Support | `support` | Müşteri destek ekibi | Hayır |
| Seller | `seller` | Satıcı/Tedarikçi | Hayır |
| Transporter | `transporter` | Nakliye hizmeti sağlayıcı | Hayır |
| Buyer | `buyer` | Alıcı (varsayılan kullanıcı rolü) | **Evet** |
| Veterinarian | `veterinarian` | Veteriner hekim | Hayır |

---

## 3. Yetki Matrisi

### 3.1 Kullanıcı Yönetimi

| İşlem | Admin | Moderator | Support | Seller | Transporter | Buyer | Vet |
|-------|:-----:|:---------:|:-------:|:------:|:-----------:|:-----:|:---:|
| Tüm kullanıcıları listele | ✅ | ✅ | ✅ | ❌ | ❌ | ❌ | ❌ |
| Kullanıcı detayı görüntüle | ✅ | ✅ | ✅ | ❌ | ❌ | ❌ | ❌ |
| Kullanıcı ban/unban | ✅ | ✅ | ❌ | ❌ | ❌ | ❌ | ❌ |
| Rol atama/kaldırma | ✅ | ❌ | ❌ | ❌ | ❌ | ❌ | ❌ |
| Satıcı doğrulama | ✅ | ✅ | ❌ | ❌ | ❌ | ❌ | ❌ |
| Nakliyeci doğrulama | ✅ | ✅ | ❌ | ❌ | ❌ | ❌ | ❌ |

### 3.2 Ürün Yönetimi

| İşlem | Admin | Moderator | Support | Seller | Transporter | Buyer | Vet |
|-------|:-----:|:---------:|:-------:|:------:|:-----------:|:-----:|:---:|
| Tüm ürünleri listele | ✅ | ✅ | ✅ | ✅* | ✅ | ✅ | ✅ |
| Ürün oluştur | ✅ | ❌ | ❌ | ✅ | ❌ | ❌ | ❌ |
| Ürün güncelle | ✅ | ❌ | ❌ | ✅* | ❌ | ❌ | ❌ |
| Ürün sil | ✅ | ✅ | ❌ | ✅* | ❌ | ❌ | ❌ |
| Ürün onayla/reddet | ✅ | ✅ | ❌ | ❌ | ❌ | ❌ | ❌ |
| Ürün durumu değiştir | ✅ | ✅ | ❌ | ✅* | ❌ | ❌ | ❌ |

*Sadece kendi ürünleri

### 3.3 Sipariş Yönetimi

| İşlem | Admin | Moderator | Support | Seller | Transporter | Buyer | Vet |
|-------|:-----:|:---------:|:-------:|:------:|:-----------:|:-----:|:---:|
| Tüm siparişleri listele | ✅ | ✅ | ✅ | ❌ | ❌ | ❌ | ❌ |
| Sipariş oluştur | ❌ | ❌ | ❌ | ❌ | ❌ | ✅ | ❌ |
| Sipariş detayı görüntüle | ✅ | ✅ | ✅ | ✅* | ✅* | ✅* | ❌ |
| Sipariş durumu güncelle | ✅ | ❌ | ✅ | ✅* | ❌ | ❌ | ❌ |
| Sipariş iptal | ✅ | ❌ | ✅ | ✅* | ❌ | ✅* | ❌ |

*Sadece kendi siparişleri (satıcı: satılan, alıcı: verilen, nakliyeci: taşınan)

### 3.4 Nakliye Yönetimi

| İşlem | Admin | Moderator | Support | Seller | Transporter | Buyer | Vet |
|-------|:-----:|:---------:|:-------:|:------:|:-----------:|:-----:|:---:|
| Nakliye talebi oluştur | ❌ | ❌ | ❌ | ✅ | ❌ | ✅ | ❌ |
| Nakliye teklifi ver | ❌ | ❌ | ❌ | ❌ | ✅ | ❌ | ❌ |
| Teklif kabul/ret | ❌ | ❌ | ❌ | ✅* | ❌ | ✅* | ❌ |
| Nakliye durumu güncelle | ✅ | ❌ | ✅ | ❌ | ✅* | ❌ | ❌ |
| Nakliye değerlendir | ❌ | ❌ | ❌ | ✅* | ❌ | ✅* | ❌ |

### 3.5 Kategori & İçerik Yönetimi

| İşlem | Admin | Moderator | Support | Seller | Transporter | Buyer | Vet |
|-------|:-----:|:---------:|:-------:|:------:|:-----------:|:-----:|:---:|
| Kategori CRUD | ✅ | ✅ | ❌ | ❌ | ❌ | ❌ | ❌ |
| Marka CRUD | ✅ | ✅ | ❌ | ❌ | ❌ | ❌ | ❌ |
| Banner CRUD | ✅ | ✅ | ❌ | ❌ | ❌ | ❌ | ❌ |
| Sayfa içeriği CRUD | ✅ | ✅ | ❌ | ❌ | ❌ | ❌ | ❌ |

### 3.6 Veteriner/Sağlık Yönetimi

| İşlem | Admin | Moderator | Support | Seller | Transporter | Buyer | Vet |
|-------|:-----:|:---------:|:-------:|:------:|:-----------:|:-----:|:---:|
| Sağlık sertifikası oluştur | ✅ | ❌ | ❌ | ❌ | ❌ | ❌ | ✅ |
| Sağlık sertifikası onayla | ✅ | ❌ | ❌ | ❌ | ❌ | ❌ | ✅ |
| Muayene raporu oluştur | ❌ | ❌ | ❌ | ❌ | ❌ | ❌ | ✅ |
| Veteriner belgeleri yönet | ✅ | ❌ | ❌ | ❌ | ❌ | ❌ | ✅* |

### 3.7 Raporlar & İstatistikler

| İşlem | Admin | Moderator | Support | Seller | Transporter | Buyer | Vet |
|-------|:-----:|:---------:|:-------:|:------:|:-----------:|:-----:|:---:|
| Platform istatistikleri | ✅ | ✅ | ❌ | ❌ | ❌ | ❌ | ❌ |
| Satış raporları | ✅ | ✅ | ❌ | ✅* | ❌ | ❌ | ❌ |
| Nakliye raporları | ✅ | ✅ | ❌ | ❌ | ✅* | ❌ | ❌ |
| Finansal raporlar | ✅ | ❌ | ❌ | ✅* | ✅* | ❌ | ❌ |

---

## 4. Implementasyon Görevleri

### Faz 1: Temel Altyapı (Öncelik: Yüksek)

#### 4.1.1 Constants Güncelleme
- [ ] `LivestockTradingConstants.cs` dosyasına tüm rolleri ekle
- Dosya: `LivestockTrading.Application/Constants/LivestockTradingConstants.cs`

```csharp
public static class Roles
{
    public const string Admin = "Admin";
    public const string Moderator = "Moderator";
    public const string Support = "Support";
    public const string Seller = "Seller";
    public const string Transporter = "Transporter";
    public const string Buyer = "Buyer";
    public const string Veterinarian = "Veterinarian";

    // Rol grupları
    public static readonly string[] AdminRoles = { Admin };
    public static readonly string[] ModeratorRoles = { Admin, Moderator };
    public static readonly string[] SupportRoles = { Admin, Moderator, Support };
    public static readonly string[] ContentManagers = { Admin, Moderator };
    public static readonly string[] AllRoles = { Admin, Moderator, Support, Seller, Transporter, Buyer, Veterinarian };
}
```

#### 4.1.2 Seed Data Oluşturma
- [ ] Rol seed data JSON dosyası oluştur
- Dosya: `Jobs/RelationalDB/MigrationJob/SeedData/roles.json`

```json
[
  {
    "id": "A1000000-0000-0000-0000-000000000001",
    "name": "Admin",
    "description": "Platform yöneticisi - Tam yetki",
    "moduleId": "LivestockTrading",
    "isActive": true
  },
  // ... diğer roller
]
```

#### 4.1.3 RoleSeeder Oluşturma
- [ ] `RoleSeeder.cs` oluştur (CountrySeeder benzeri)
- Dosya: `Jobs/RelationalDB/MigrationJob/Seeders/RoleSeeder.cs`

#### 4.1.4 Migration Job Güncelleme
- [ ] `Program.cs`'e RoleSeeder çağrısı ekle
- Dosya: `Jobs/RelationalDB/MigrationJob/Program.cs`

---

### Faz 2: Authorization Altyapısı (Öncelik: Yüksek)

#### 4.2.1 Permission Enum Oluşturma
- [ ] Tüm izinleri içeren enum oluştur
- Dosya: `LivestockTrading.Domain/Enums/Permissions.cs`

```csharp
public enum Permission
{
    // Kullanıcı Yönetimi
    UsersView = 100,
    UsersCreate = 101,
    UsersUpdate = 102,
    UsersDelete = 103,
    UsersBan = 104,

    // Ürün Yönetimi
    ProductsView = 200,
    ProductsCreate = 201,
    ProductsUpdate = 202,
    ProductsDelete = 203,
    ProductsApprove = 204,

    // ... diğer izinler
}
```

#### 4.2.2 RolePermission Mapping
- [ ] Her rol için izin mapping'i oluştur
- Dosya: `LivestockTrading.Application/Authorization/RolePermissions.cs`

```csharp
public static class RolePermissions
{
    public static readonly Dictionary<string, Permission[]> Mappings = new()
    {
        [Roles.Admin] = Enum.GetValues<Permission>(), // Tüm izinler
        [Roles.Moderator] = new[] { Permission.ProductsApprove, ... },
        [Roles.Seller] = new[] { Permission.ProductsCreate, ... },
        // ...
    };
}
```

#### 4.2.3 Authorization Attribute/Filter
- [ ] Yetki kontrolü için custom attribute oluştur
- Dosya: `LivestockTrading.Application/Authorization/RequirePermissionAttribute.cs`

```csharp
[AttributeUsage(AttributeTargets.Class)]
public class RequirePermissionAttribute : Attribute
{
    public Permission[] Permissions { get; }
    public bool RequireAll { get; set; } = false;

    public RequirePermissionAttribute(params Permission[] permissions)
    {
        Permissions = permissions;
    }
}
```

---

### Faz 3: Verificator Güncellemeleri (Öncelik: Orta)

Her endpoint'in Verificator.cs dosyası güncellenmeli:

#### 4.3.1 Ürün Endpoint'leri
- [ ] `Products/Commands/Create/Verificator.cs` - Seller rolü gerekli
- [ ] `Products/Commands/Update/Verificator.cs` - Seller (kendi ürünü) veya Admin
- [ ] `Products/Commands/Delete/Verificator.cs` - Seller (kendi ürünü) veya Admin/Moderator
- [ ] `Products/Commands/Approve/Verificator.cs` - Admin veya Moderator (YENİ ENDPOINT)
- [ ] `Products/Queries/All/Verificator.cs` - Public (herkes görebilir)
- [ ] `Products/Queries/Detail/Verificator.cs` - Public

#### 4.3.2 Sipariş Endpoint'leri
- [ ] `Orders/Commands/Create/Verificator.cs` - Buyer rolü gerekli
- [ ] `Orders/Commands/Update/Verificator.cs` - Seller/Admin/Support (kendi siparişi)
- [ ] `Orders/Commands/Cancel/Verificator.cs` - Buyer (kendi siparişi) veya Admin/Support
- [ ] `Orders/Queries/All/Verificator.cs` - Admin/Moderator/Support veya kendi siparişleri
- [ ] `Orders/Queries/Detail/Verificator.cs` - İlgili taraflar (Buyer/Seller/Transporter)

#### 4.3.3 Kategori Endpoint'leri
- [ ] `Categories/Commands/Create/Verificator.cs` - Admin/Moderator
- [ ] `Categories/Commands/Update/Verificator.cs` - Admin/Moderator
- [ ] `Categories/Commands/Delete/Verificator.cs` - Admin/Moderator
- [ ] `Categories/Queries/*/Verificator.cs` - Public

#### 4.3.4 Nakliye Endpoint'leri
- [ ] `TransportRequests/Commands/Create/Verificator.cs` - Seller/Buyer
- [ ] `TransportOffers/Commands/Create/Verificator.cs` - Transporter
- [ ] `TransportOffers/Commands/Accept/Verificator.cs` - Talep sahibi (Seller/Buyer)
- [ ] Tüm Transporter endpoint'leri - İlgili roller

#### 4.3.5 Satıcı Endpoint'leri
- [ ] `Sellers/Commands/Create/Verificator.cs` - Authenticated user (Buyer → Seller başvurusu)
- [ ] `Sellers/Commands/Update/Verificator.cs` - Seller (kendi profili) veya Admin
- [ ] `Sellers/Commands/Verify/Verificator.cs` - Admin/Moderator (YENİ ENDPOINT)
- [ ] `Sellers/Queries/*/Verificator.cs` - Public

#### 4.3.6 Nakliyeci Endpoint'leri
- [ ] `Transporters/Commands/Create/Verificator.cs` - Authenticated user (başvuru)
- [ ] `Transporters/Commands/Update/Verificator.cs` - Transporter (kendi) veya Admin
- [ ] `Transporters/Commands/Verify/Verificator.cs` - Admin/Moderator (YENİ ENDPOINT)

---

### Faz 4: Yeni Endpoint'ler (Öncelik: Orta)

#### 4.4.1 Ürün Onaylama
- [ ] `Products/Commands/Approve/` - 6 dosya (Handler, Models, DataAccess, Mapper, Validator, Verificator)
- [ ] `Products/Commands/Reject/` - 6 dosya

#### 4.4.2 Satıcı/Nakliyeci Doğrulama
- [ ] `Sellers/Commands/Verify/` - 6 dosya
- [ ] `Sellers/Commands/Suspend/` - 6 dosya
- [ ] `Transporters/Commands/Verify/` - 6 dosya
- [ ] `Transporters/Commands/Suspend/` - 6 dosya

#### 4.4.3 Rol Yönetimi (IAM Modülünde)
- [ ] `Roles/Commands/AssignToUser/` - 6 dosya
- [ ] `Roles/Commands/RemoveFromUser/` - 6 dosya
- [ ] `Roles/Queries/GetUserRoles/` - 6 dosya

---

### Faz 5: Kayıt Akışı Güncellemesi (Öncelik: Orta)

#### 4.5.1 Kullanıcı Kaydı
- [ ] Yeni kullanıcıya otomatik "Buyer" rolü ata
- Dosya: `BaseModules.IAM.Application/RequestHandlers/Users/Commands/Create/Handler.cs`

#### 4.5.2 Satıcı Başvurusu
- [ ] Satıcı başvurusu akışı (Buyer → Seller)
- [ ] Onay sonrası rol ataması
- [ ] E-posta bildirimi

#### 4.5.3 Nakliyeci Başvurusu
- [ ] Nakliyeci başvurusu akışı (Buyer → Transporter)
- [ ] Onay sonrası rol ataması
- [ ] E-posta bildirimi

---

### Faz 6: Frontend Entegrasyonu (Öncelik: Düşük)

#### 4.6.1 API-INTEGRATION.md Güncelleme
- [ ] Rol bazlı endpoint erişim dokümantasyonu
- [ ] JWT rol claim formatı
- [ ] Yetki hata kodları

#### 4.6.2 Error Codes
- [ ] DomainErrors.cs'e yetki hataları ekle
```csharp
public static class AuthorizationErrors
{
    public static string InsufficientPermission = "Bu işlem için yetkiniz bulunmamaktadır.";
    public static string RoleRequired = "Bu işlem için {0} rolü gereklidir.";
    public static string SellerNotVerified = "Satıcı hesabınız henüz doğrulanmamış.";
    public static string TransporterNotVerified = "Nakliyeci hesabınız henüz doğrulanmamış.";
}
```

---

## 5. Test Senaryoları

### 5.1 Rol Atama Testleri
- [ ] Yeni kullanıcı → Buyer rolü otomatik atanıyor mu?
- [ ] Admin → Kullanıcıya rol atabiliyor mu?
- [ ] Non-Admin → Rol atayamıyor mu?

### 5.2 Ürün Yetki Testleri
- [ ] Seller → Ürün oluşturabiliyor mu?
- [ ] Buyer → Ürün oluşturamıyor mu?
- [ ] Seller → Başkasının ürününü güncelleyemiyor mu?
- [ ] Moderator → Ürün onaylayabiliyor mu?

### 5.3 Sipariş Yetki Testleri
- [ ] Buyer → Sipariş verebiliyor mu?
- [ ] Seller → Kendi siparişlerini görebiliyor mu?
- [ ] Seller → Başkasının siparişini göremiyor mu?

### 5.4 Nakliye Yetki Testleri
- [ ] Transporter → Teklif verebiliyor mu?
- [ ] Non-Transporter → Teklif veremiyor mu?
- [ ] Talep sahibi → Teklifi kabul edebiliyor mu?

---

## 6. Dosya Listesi (Oluşturulacak/Güncellenecek)

### Yeni Dosyalar
```
LivestockTrading.Application/
├── Authorization/
│   ├── Permissions.cs
│   ├── RolePermissions.cs
│   └── RequirePermissionAttribute.cs
├── RequestHandlers/
│   ├── Products/Commands/Approve/
│   │   ├── Handler.cs
│   │   ├── Models.cs
│   │   ├── DataAccess.cs
│   │   ├── Mapper.cs
│   │   ├── Validator.cs
│   │   └── Verificator.cs
│   ├── Products/Commands/Reject/
│   ├── Sellers/Commands/Verify/
│   ├── Sellers/Commands/Suspend/
│   ├── Transporters/Commands/Verify/
│   └── Transporters/Commands/Suspend/

Jobs/RelationalDB/MigrationJob/
├── SeedData/
│   └── roles.json
├── Seeders/
│   └── RoleSeeder.cs

LivestockTrading.Domain/
└── Errors/
    └── DomainErrors.cs (AuthorizationErrors ekleme)
```

### Güncellenecek Dosyalar
```
LivestockTrading.Application/
├── Constants/
│   └── LivestockTradingConstants.cs
├── RequestHandlers/
│   ├── Products/*/Verificator.cs (tümü)
│   ├── Orders/*/Verificator.cs (tümü)
│   ├── Categories/*/Verificator.cs (tümü)
│   ├── Sellers/*/Verificator.cs (tümü)
│   ├── Transporters/*/Verificator.cs (tümü)
│   └── ... (diğer tüm Verificator'lar)

BaseModules.IAM.Application/
├── RequestHandlers/
│   └── Users/Commands/Create/Handler.cs (otomatik Buyer rolü)

Jobs/RelationalDB/MigrationJob/
└── Program.cs (RoleSeeder çağrısı)

_doc/
└── API-INTEGRATION.md
```

---

## 7. Öncelik Sıralaması

| Öncelik | Görev | Tahmini Efor |
|---------|-------|--------------|
| 1 | Constants güncelleme | 30 dk |
| 2 | Seed data & RoleSeeder | 2 saat |
| 3 | Permission enum & mapping | 2 saat |
| 4 | Authorization attribute | 1 saat |
| 5 | Verificator güncellemeleri | 4-6 saat |
| 6 | Yeni endpoint'ler | 4-6 saat |
| 7 | Kayıt akışı güncelleme | 2 saat |
| 8 | Test & dokümantasyon | 2-3 saat |

**Toplam Tahmini Süre: 18-22 saat**

---

## 8. Frontend Entegrasyonu

### 8.1 Rol Bilgisi Endpoint'leri (IAM Modülü)

Frontend'in rol bilgilerine erişmesi için aşağıdaki endpoint'ler gerekli:

#### 8.1.1 Mevcut Kullanıcı Bilgisi (Roller Dahil)
```
POST /iam/Users/Me
Authorization: Bearer {token}

Response:
{
  "data": {
    "id": "user-guid",
    "email": "user@example.com",
    "displayName": "John Doe",
    "roles": ["Buyer", "Seller"],
    "permissions": ["products.create", "products.update", "orders.create"]
  }
}
```

#### 8.1.2 Tüm Rolleri Listele
```
POST /iam/Roles/All
Authorization: Bearer {token} (Public - herkes görebilir)

Response:
{
  "data": [
    {
      "id": "guid-1",
      "name": "Admin",
      "description": "Platform yöneticisi - Tam yetki",
      "isDefault": false
    },
    {
      "id": "guid-2",
      "name": "Buyer",
      "description": "Alıcı - Varsayılan kullanıcı rolü",
      "isDefault": true
    }
  ]
}
```

#### 8.1.3 Rol Yetkileri (Admin için)
```
POST /iam/Roles/Permissions
Authorization: Bearer {token} (Admin only)

Response:
{
  "data": {
    "Admin": ["*"],
    "Moderator": ["products.approve", "products.reject", "users.view", "categories.manage"],
    "Seller": ["products.create", "products.update", "products.delete", "orders.manage"],
    "Transporter": ["transport.offer", "transport.manage"],
    "Buyer": ["orders.create", "favorites.manage", "reviews.create"],
    "Support": ["tickets.view", "tickets.respond", "users.view"],
    "Veterinarian": ["health.certify", "health.report"]
  }
}
```

### 8.2 JWT Token Yapısı

Login başarılı olduğunda dönen JWT token'da roller bulunur:

```javascript
// JWT Payload
{
  "nameid": "user-guid",
  "given_name": "username",
  "unique_name": "John Doe",
  "email": "user@example.com",
  "tenantId": "tenant-guid",
  "role": ["Buyer", "Seller"],  // Birden fazla rol olabilir
  "exp": 1699999999
}
```

### 8.3 Frontend Kullanım Örnekleri

#### Token Decode & Rol Kontrolü
```typescript
// utils/auth.ts
export function decodeToken(token: string): JwtPayload {
  const base64Url = token.split('.')[1];
  const base64 = base64Url.replace(/-/g, '+').replace(/_/g, '/');
  return JSON.parse(window.atob(base64));
}

export function getUserRoles(token: string): string[] {
  const payload = decodeToken(token);
  // role tek string veya array olabilir
  if (Array.isArray(payload.role)) {
    return payload.role;
  }
  return payload.role ? [payload.role] : [];
}

export function hasRole(token: string, role: string): boolean {
  const roles = getUserRoles(token);
  return roles.includes(role);
}

export function hasAnyRole(token: string, requiredRoles: string[]): boolean {
  const roles = getUserRoles(token);
  return requiredRoles.some(r => roles.includes(r));
}
```

#### React Context Örneği
```typescript
// contexts/AuthContext.tsx
interface AuthContextType {
  user: User | null;
  roles: string[];
  hasRole: (role: string) => boolean;
  hasPermission: (permission: string) => boolean;
}

export const AuthProvider: React.FC = ({ children }) => {
  const [user, setUser] = useState<User | null>(null);
  const [roles, setRoles] = useState<string[]>([]);

  useEffect(() => {
    const token = localStorage.getItem('token');
    if (token) {
      setRoles(getUserRoles(token));
      // /Users/Me endpoint'inden detaylı bilgi çek
      fetchUserProfile();
    }
  }, []);

  const hasRole = (role: string) => roles.includes(role);

  const hasPermission = (permission: string) => {
    // RolePermissions mapping kullanarak kontrol
    return roles.some(role =>
      RolePermissions[role]?.includes(permission) ||
      RolePermissions[role]?.includes('*')
    );
  };

  return (
    <AuthContext.Provider value={{ user, roles, hasRole, hasPermission }}>
      {children}
    </AuthContext.Provider>
  );
};
```

#### Conditional Rendering
```tsx
// components/Navbar.tsx
function Navbar() {
  const { hasRole, hasPermission } = useAuth();

  return (
    <nav>
      <Link to="/">Ana Sayfa</Link>

      {/* Sadece Seller görebilir */}
      {hasRole('Seller') && (
        <Link to="/products/new">Ürün Ekle</Link>
      )}

      {/* Admin veya Moderator görebilir */}
      {(hasRole('Admin') || hasRole('Moderator')) && (
        <Link to="/admin/products">Ürün Onaylama</Link>
      )}

      {/* Permission bazlı kontrol */}
      {hasPermission('categories.manage') && (
        <Link to="/admin/categories">Kategori Yönetimi</Link>
      )}

      {/* Transporter görebilir */}
      {hasRole('Transporter') && (
        <Link to="/transport/offers">Nakliye Teklifleri</Link>
      )}
    </nav>
  );
}
```

#### Protected Route
```tsx
// components/ProtectedRoute.tsx
interface ProtectedRouteProps {
  roles?: string[];
  permissions?: string[];
  children: React.ReactNode;
}

function ProtectedRoute({ roles, permissions, children }: ProtectedRouteProps) {
  const { hasRole, hasPermission, isAuthenticated } = useAuth();

  if (!isAuthenticated) {
    return <Navigate to="/login" />;
  }

  if (roles && !roles.some(r => hasRole(r))) {
    return <Navigate to="/unauthorized" />;
  }

  if (permissions && !permissions.some(p => hasPermission(p))) {
    return <Navigate to="/unauthorized" />;
  }

  return <>{children}</>;
}

// Kullanım
<Route
  path="/admin/products"
  element={
    <ProtectedRoute roles={['Admin', 'Moderator']}>
      <ProductApprovalPage />
    </ProtectedRoute>
  }
/>
```

### 8.4 Frontend Constants (Hardcoded Permissions)

```typescript
// constants/roles.ts
export const Roles = {
  ADMIN: 'Admin',
  MODERATOR: 'Moderator',
  SUPPORT: 'Support',
  SELLER: 'Seller',
  TRANSPORTER: 'Transporter',
  BUYER: 'Buyer',
  VETERINARIAN: 'Veterinarian',
} as const;

// constants/permissions.ts
export const RolePermissions: Record<string, string[]> = {
  [Roles.ADMIN]: ['*'],
  [Roles.MODERATOR]: [
    'products.approve',
    'products.reject',
    'users.view',
    'users.ban',
    'categories.manage',
    'brands.manage',
  ],
  [Roles.SUPPORT]: [
    'tickets.view',
    'tickets.respond',
    'users.view',
    'orders.view',
  ],
  [Roles.SELLER]: [
    'products.create',
    'products.update',
    'products.delete',
    'orders.manage',
    'farms.manage',
  ],
  [Roles.TRANSPORTER]: [
    'transport.offer',
    'transport.manage',
  ],
  [Roles.BUYER]: [
    'orders.create',
    'favorites.manage',
    'reviews.create',
  ],
  [Roles.VETERINARIAN]: [
    'health.certify',
    'health.report',
  ],
};
```

---

## 9. Notlar

1. **Çoklu Rol Desteği**: Bir kullanıcı birden fazla role sahip olabilir (örn: Seller + Transporter)
2. **Rol Hiyerarşisi**: Admin > Moderator > Support > Seller/Transporter > Buyer
3. **Varsayılan Rol**: Yeni kayıt olan herkes otomatik "Buyer" rolü alır
4. **Doğrulama Gerekliliği**: Seller ve Transporter rolleri için doğrulama gerekli
5. **JWT Claims**: Roller JWT token'da `"LivestockTrading.RoleName"` formatında saklanır
