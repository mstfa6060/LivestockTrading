# Karar 4 — Migration Planı (7 Alt-Karar)

**Status:** FINAL
**Karar tarihi:** Planning session #4 (alt-kararlar 4a-4g)

## İlişkili Kararlar

- **Üst:** [Karar 1 — Solution yapısı](01-architecture.md), [Karar 2 — Modül listesi](02-modules-list.md), [Karar 3 — Domain patterns](03-domain-patterns.md)
- **Alt:** [Karar 5 modül detayları](05-modules/), [Karar 7 — Operations](07-operations.md)

---

## 4a — Migration Sırası

### Logical Dependency Map

Detay: [02-modules-list.md / Bölüm 3](02-modules-list.md#bölüm-3-wave-planı-dag).

**Cross-modül FK yok** (Karar 3d) — schema oluşturma sırası teknik olarak parallel; ama seed verisi mantıksal olarak ardışık (Catalog → diğerleri).

### Wave Plan Final

```
Wave 0 — Infrastructure (init.sql, separate)
    PostGIS + pg_trgm + btree_gist extensions
    7 schema (identity/accounts/carrier/catalog/listings/marketplace/messaging/notifications/subscription/admin) + public
    2 role (livestock_app, livestock_migrator)

Wave 1 — Catalog (foundation)
    Country/Currency/Language/CertificationType/Brand/Location/BorderRule seed
    
Wave 2 — Identity
    User + RefreshToken + UserDevice + UserExternalLogin + UserRole + UserConsent + PhoneVerificationTicket
    dev seed: 1 admin user

Wave 3+4 (parallel prod / sequential dev)
    ├─ Wave 3: Accounts (Seller + Farm + VetProfile + Review)
    └─ Wave 4: Listings (Listing + SavedSearch)
    
Wave 5 (parallel)
    ├─ Carrier (Carrier + Shipment + CarrierOffer)
    └─ Subscription (Plan + Subscription + Invoice + BoostPackage + BoostCampaign)
    Plan seed: Free/Standard/Pro/Enterprise + commission rules
    BoostPackage seed: top-row, showcase, urgent-label

Wave 6 (parallel)
    ├─ Marketplace (Offer + Deal + Favorite + Dispute)
    ├─ Messaging (Conversation + Message + ...)
    └─ Notifications (NotificationPreference + InAppNotification + NotificationTemplate seed)

Wave 7
    Admin (FeatureFlag + ScheduledReport + SystemVersion + ImpersonationSession + AdminAuditLog)
    FeatureFlag seed başlangıç
```

### Parallel-Safe Analiz (Özet)

| Wave | Prod Parallel | Dev Parallel | Sebep |
|---|---|---|---|
| 3+4 | ✓ | ✗ | Cross-modül FK yok; dev sample Listings için Seller bekliyor |
| 5 (Carrier + Subscription) | ✓ | ✓ | Bağımsız modüller |
| 6 (Marketplace + Messaging + Notifications) | ✓ | ✓ | 3 ayrı schema |
| 7 (Admin) | Tek | Tek | Son, cross-module aggregator |

### Admin Wave 7 Gerekçeleri

1. Cross-module read dependency (dashboard aggregate)
2. Cross-module write pattern (`IAdminXCommands`) — diğer modüllerin command surface'i hazır olmalı
3. Event consumer registration — tüm Public event tipleri Shared/Events/ altında var olmalı
4. Schema migration bağımsızlığı (pratik operasyonel sadelik)

---

## 4b — PostgreSQL Schema-Per-Modül

### Schema İsimlendirme

**Kural:** lowercase, modül adıyla aynı.

| Modül | Schema |
|---|---|
| Identity | `identity` |
| Accounts | `accounts` |
| Catalog | `catalog` |
| Listings | `listings` |
| Marketplace | `marketplace` |
| Messaging | `messaging` |
| Notifications | `notifications` |
| Carrier | `carrier` |
| Subscription | `subscription` |
| Admin | `admin` |

### Public Schema İçeriği

**Public'te KALACAK:**
- PostGIS sistem tabloları (`spatial_ref_sys`, fonksiyonlar)
- `pg_trgm`, `btree_gist` extension fonksiyonları

**Public'te OLMAYACAK:**
- Uygulama tabloları (modül schema'larına)
- EF migration history (per-schema)
- Seed-only reference data (Catalog'a)
- Audit log, outbox tabloları (her modülün kendi schema'sı)

### EF Core Schema Configuration

```csharp
public class IdentityDbContext : DbContext
{
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("identity");
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(IdentityDbContext).Assembly);
    }
}

// DI:
services.AddDbContext<IdentityDbContext>(options =>
    options.UseNpgsql(connStr, npgsql =>
    {
        npgsql.MigrationsHistoryTable("__EFMigrationsHistory", "identity");
        npgsql.UseNetTopologySuite();
    }));
```

### Cross-Schema Query Politikası

**Aynı modül içinde:** Yok (her modül tek schema).

**Cross-modül:** YASAK (Karar 3d FK yasağı + DbContext izolasyonu).

**Kasıtlı istisna:** Cross-modül raporlama (admin/analytics) Dapper raw SQL, `// CROSS-MODULE READ — design exception` yorumu zorunlu.

### search_path Stratejisi — Hibrit

```
Connection string: "Search Path=identity,public"
EF Core: HasDefaultSchema("identity") + qualified SQL üretiyor
```

**Belt-and-suspenders:** EF qualified isim güvenli; search_path Dapper raw SQL için defensive.

### Permission Strategy

**2 PostgreSQL Role:**

| Role | Kullanıcı | Yetki |
|---|---|---|
| `livestock_app` | Runtime API + Worker | DML (SELECT/INSERT/UPDATE/DELETE) — her schema |
| `livestock_migrator` | Migration runner (CI) | DDL (CREATE/ALTER/DROP) + DML |

Per-modül user reddedildi — kod katmanı DbContext izolasyonu zaten yeterli.

### Migration History Per-Schema

```
identity.__EFMigrationsHistory
accounts.__EFMigrationsHistory
... (10 schema)
```

Sebep: modül izolasyonu, `DROP SCHEMA X CASCADE` migration history'yi de götürür, `pg_dump --schema=X` complete.

### Wave 0 init.sql — Tam İçerik

**Dosya yerleşimi:** `_devops/db/init.sql.template` + `_devops/db/init.sh` wrapper.

#### Extension Seçimi — Açıklamalar

| Extension | Yüklü mü | Sebep |
|---|---|---|
| `postgis` | ✓ Yüklü | Spatial geometry/geography (Listings, Carrier, Accounts, Marketplace, Catalog) |
| `pg_trgm` | ✓ Yüklü | Trigram similarity için FTS keyword search (Listings Faz 1; Faz 2 Meilisearch'e geçince opsiyonel) |
| `btree_gist` | ✓ Yüklü | **Composite GIST index** mixing B-tree + GIST sütunları. Karar 4e Madde 4 — `ix_listings_country_location_gist` gibi `country_code = 'TR' AND ST_DWithin(...)` query'lerinde tek composite index ile B-tree filter + GIST spatial birlikte |
| `uuid-ossp` / `pgcrypto` | ✗ Yüklü değil | **Karar:** Tüm Guid ID'ler client-side `.NET 9+ Guid.CreateVersion7()` ile üretiliyor (Karar 3a AR factory pattern'lerinde kullanılıyor). DB-side UUID üretimine ihtiyaç yok |

#### search_path Stratejisi — init.sql'de Yok

init.sql'de **search_path ayarı YOK**, sebebi:
- init.sql DDL-only, superuser (`postgres`) tarafından bootstrap'ta tek seferlik çalışıyor
- Tüm CREATE statement'lar **schema-qualified** (örn. `CREATE SCHEMA identity`)
- Runtime'da her modülün connection string'i kendi search_path'ini taşıyor (Karar 4f — `Search Path=identity,public`)
- EF Core `HasDefaultSchema(...)` ile qualified SQL üretiyor (Karar 4b — hibrit strateji)
- search_path defensive katman, init.sql'de gereksiz

#### Password Substitution — envsubst Wrapper

`${MIGRATOR_PASSWORD}` ve `${APP_PASSWORD}` template placeholder. Runtime substitution `init.sh` script'i tarafından:

```bash
#!/bin/bash
# _devops/db/init.sh — Wave 0 PostgreSQL bootstrap wrapper
# Runs once at first container start via /docker-entrypoint-initdb.d/00-init.sh
set -euo pipefail

: "${MIGRATOR_PASSWORD:?MIGRATOR_PASSWORD env var required}"
: "${APP_PASSWORD:?APP_PASSWORD env var required}"

# Substitute env vars in template → temp file
envsubst < /docker-entrypoint-initdb.d/init.sql.template > /tmp/init.sql

# Execute as postgres superuser
psql -v ON_ERROR_STOP=1 \
  --username "$POSTGRES_USER" \
  --dbname "$POSTGRES_DB" \
  -f /tmp/init.sql

# Cleanup substituted file (contains plaintext passwords)
shred -u /tmp/init.sql 2>/dev/null || rm -f /tmp/init.sql
```

Docker Compose mount:

```yaml
postgres:
  image: postgis/postgis:17-3.5
  environment:
    POSTGRES_DB: livestock_trading
    POSTGRES_USER: postgres
    POSTGRES_PASSWORD: ${POSTGRES_SUPER_PASSWORD}
    MIGRATOR_PASSWORD: ${LT__DATABASE__MIGRATORPASSWORD}
    APP_PASSWORD: ${LT__DATABASE__APPPASSWORD}
  volumes:
    - ./_devops/db/init.sh:/docker-entrypoint-initdb.d/00-init.sh:ro
    - ./_devops/db/init.sql.template:/docker-entrypoint-initdb.d/init.sql.template:ro
```

Naming `00-init.sh` ensures alphabetical first run. PostgreSQL Docker image entrypoint `.sh` script'leri executable yapıp çalıştırıyor.

**Alternatif (psql -v):** `psql -v migrator_pwd="$MIGRATOR_PASSWORD" -f init.sql` ve SQL içinde `:'migrator_pwd'` — envsubst'a göre daha açık ama placeholder format farklı. Hibrit production-friendly tercih envsubst.

#### Tam init.sql.template İçeriği

```sql
-- ============================================================================
-- Livestock Trading — Wave 0 Database Initialization
-- ============================================================================
-- Runs once at first PostgreSQL container start (via /docker-entrypoint-initdb.d/).
-- Subsequent runs no-op (IF NOT EXISTS guards).
-- 
-- Password substitution: ${MIGRATOR_PASSWORD} and ${APP_PASSWORD} are placeholders;
-- replaced by envsubst in init.sh wrapper before psql execution.
-- ============================================================================

-- ----------------------------------------------------------------------------
-- 1. Extensions (public schema)
-- ----------------------------------------------------------------------------

-- Spatial geometry/geography types and operations
CREATE EXTENSION IF NOT EXISTS postgis;

-- Trigram similarity for FTS keyword search (Listings Faz 1)
CREATE EXTENSION IF NOT EXISTS pg_trgm;

-- Composite GIST indexes mixing B-tree and GIST columns
-- Required for: ix_listings_country_location_gist (Karar 4e Madde 4)
CREATE EXTENSION IF NOT EXISTS btree_gist;

-- NOTE: uuid-ossp / pgcrypto NOT installed.
-- All Guid IDs generated client-side via Guid.CreateVersion7() (.NET 9+).
-- DB-side UUID generation not needed (Karar 3a AR factories use Guid v7).

-- ----------------------------------------------------------------------------
-- 2. Schemas (10 module + public retained for PostGIS)
-- ----------------------------------------------------------------------------

CREATE SCHEMA IF NOT EXISTS identity;
CREATE SCHEMA IF NOT EXISTS accounts;
CREATE SCHEMA IF NOT EXISTS catalog;
CREATE SCHEMA IF NOT EXISTS listings;
CREATE SCHEMA IF NOT EXISTS carrier;
CREATE SCHEMA IF NOT EXISTS marketplace;
CREATE SCHEMA IF NOT EXISTS messaging;
CREATE SCHEMA IF NOT EXISTS notifications;
CREATE SCHEMA IF NOT EXISTS subscription;
CREATE SCHEMA IF NOT EXISTS admin;

-- ----------------------------------------------------------------------------
-- 3. Roles (Karar 4b — 2-role least privilege)
-- ----------------------------------------------------------------------------
-- livestock_migrator: DDL (CREATE/ALTER/DROP) + DML; used by EF migrations + SeedRunner
-- livestock_app:       DML only (SELECT/INSERT/UPDATE/DELETE); runtime API + Workers
-- ----------------------------------------------------------------------------

CREATE ROLE livestock_migrator WITH LOGIN PASSWORD '${MIGRATOR_PASSWORD}';
CREATE ROLE livestock_app WITH LOGIN PASSWORD '${APP_PASSWORD}';

-- ----------------------------------------------------------------------------
-- 4. Schema-level grants
-- ----------------------------------------------------------------------------

-- Migrator: full DDL on all 10 module schemas
GRANT ALL ON SCHEMA identity, accounts, catalog, listings, carrier,
                marketplace, messaging, notifications, subscription, admin 
              TO livestock_migrator;

-- App: USAGE only on module schemas (no DDL, no CREATE)
GRANT USAGE ON SCHEMA identity, accounts, catalog, listings, carrier,
                  marketplace, messaging, notifications, subscription, admin 
              TO livestock_app;

-- App: PostGIS function access (public schema)
GRANT USAGE ON SCHEMA public TO livestock_app;
GRANT SELECT ON spatial_ref_sys TO livestock_app;

-- ----------------------------------------------------------------------------
-- 5. Default privileges (auto-apply to future tables/sequences/functions)
-- ----------------------------------------------------------------------------
-- For each of 10 schemas × 3 statements (TABLES + SEQUENCES + FUNCTIONS) = 30 statements
-- 
-- TABLES:    app does SELECT/INSERT/UPDATE/DELETE
-- SEQUENCES: app uses (EF SERIAL / IDENTITY columns require USAGE; SELECT for currval)
-- FUNCTIONS: app executes (Faz 2 admin-defined functions; defensive grant)
-- ----------------------------------------------------------------------------

-- identity
ALTER DEFAULT PRIVILEGES FOR ROLE livestock_migrator IN SCHEMA identity
  GRANT SELECT, INSERT, UPDATE, DELETE ON TABLES TO livestock_app;
ALTER DEFAULT PRIVILEGES FOR ROLE livestock_migrator IN SCHEMA identity
  GRANT USAGE, SELECT ON SEQUENCES TO livestock_app;
ALTER DEFAULT PRIVILEGES FOR ROLE livestock_migrator IN SCHEMA identity
  GRANT EXECUTE ON FUNCTIONS TO livestock_app;

-- accounts
ALTER DEFAULT PRIVILEGES FOR ROLE livestock_migrator IN SCHEMA accounts
  GRANT SELECT, INSERT, UPDATE, DELETE ON TABLES TO livestock_app;
ALTER DEFAULT PRIVILEGES FOR ROLE livestock_migrator IN SCHEMA accounts
  GRANT USAGE, SELECT ON SEQUENCES TO livestock_app;
ALTER DEFAULT PRIVILEGES FOR ROLE livestock_migrator IN SCHEMA accounts
  GRANT EXECUTE ON FUNCTIONS TO livestock_app;

-- catalog
ALTER DEFAULT PRIVILEGES FOR ROLE livestock_migrator IN SCHEMA catalog
  GRANT SELECT, INSERT, UPDATE, DELETE ON TABLES TO livestock_app;
ALTER DEFAULT PRIVILEGES FOR ROLE livestock_migrator IN SCHEMA catalog
  GRANT USAGE, SELECT ON SEQUENCES TO livestock_app;
ALTER DEFAULT PRIVILEGES FOR ROLE livestock_migrator IN SCHEMA catalog
  GRANT EXECUTE ON FUNCTIONS TO livestock_app;

-- listings
ALTER DEFAULT PRIVILEGES FOR ROLE livestock_migrator IN SCHEMA listings
  GRANT SELECT, INSERT, UPDATE, DELETE ON TABLES TO livestock_app;
ALTER DEFAULT PRIVILEGES FOR ROLE livestock_migrator IN SCHEMA listings
  GRANT USAGE, SELECT ON SEQUENCES TO livestock_app;
ALTER DEFAULT PRIVILEGES FOR ROLE livestock_migrator IN SCHEMA listings
  GRANT EXECUTE ON FUNCTIONS TO livestock_app;

-- carrier
ALTER DEFAULT PRIVILEGES FOR ROLE livestock_migrator IN SCHEMA carrier
  GRANT SELECT, INSERT, UPDATE, DELETE ON TABLES TO livestock_app;
ALTER DEFAULT PRIVILEGES FOR ROLE livestock_migrator IN SCHEMA carrier
  GRANT USAGE, SELECT ON SEQUENCES TO livestock_app;
ALTER DEFAULT PRIVILEGES FOR ROLE livestock_migrator IN SCHEMA carrier
  GRANT EXECUTE ON FUNCTIONS TO livestock_app;

-- marketplace
ALTER DEFAULT PRIVILEGES FOR ROLE livestock_migrator IN SCHEMA marketplace
  GRANT SELECT, INSERT, UPDATE, DELETE ON TABLES TO livestock_app;
ALTER DEFAULT PRIVILEGES FOR ROLE livestock_migrator IN SCHEMA marketplace
  GRANT USAGE, SELECT ON SEQUENCES TO livestock_app;
ALTER DEFAULT PRIVILEGES FOR ROLE livestock_migrator IN SCHEMA marketplace
  GRANT EXECUTE ON FUNCTIONS TO livestock_app;

-- messaging
ALTER DEFAULT PRIVILEGES FOR ROLE livestock_migrator IN SCHEMA messaging
  GRANT SELECT, INSERT, UPDATE, DELETE ON TABLES TO livestock_app;
ALTER DEFAULT PRIVILEGES FOR ROLE livestock_migrator IN SCHEMA messaging
  GRANT USAGE, SELECT ON SEQUENCES TO livestock_app;
ALTER DEFAULT PRIVILEGES FOR ROLE livestock_migrator IN SCHEMA messaging
  GRANT EXECUTE ON FUNCTIONS TO livestock_app;

-- notifications
ALTER DEFAULT PRIVILEGES FOR ROLE livestock_migrator IN SCHEMA notifications
  GRANT SELECT, INSERT, UPDATE, DELETE ON TABLES TO livestock_app;
ALTER DEFAULT PRIVILEGES FOR ROLE livestock_migrator IN SCHEMA notifications
  GRANT USAGE, SELECT ON SEQUENCES TO livestock_app;
ALTER DEFAULT PRIVILEGES FOR ROLE livestock_migrator IN SCHEMA notifications
  GRANT EXECUTE ON FUNCTIONS TO livestock_app;

-- subscription
ALTER DEFAULT PRIVILEGES FOR ROLE livestock_migrator IN SCHEMA subscription
  GRANT SELECT, INSERT, UPDATE, DELETE ON TABLES TO livestock_app;
ALTER DEFAULT PRIVILEGES FOR ROLE livestock_migrator IN SCHEMA subscription
  GRANT USAGE, SELECT ON SEQUENCES TO livestock_app;
ALTER DEFAULT PRIVILEGES FOR ROLE livestock_migrator IN SCHEMA subscription
  GRANT EXECUTE ON FUNCTIONS TO livestock_app;

-- admin
ALTER DEFAULT PRIVILEGES FOR ROLE livestock_migrator IN SCHEMA admin
  GRANT SELECT, INSERT, UPDATE, DELETE ON TABLES TO livestock_app;
ALTER DEFAULT PRIVILEGES FOR ROLE livestock_migrator IN SCHEMA admin
  GRANT USAGE, SELECT ON SEQUENCES TO livestock_app;
ALTER DEFAULT PRIVILEGES FOR ROLE livestock_migrator IN SCHEMA admin
  GRANT EXECUTE ON FUNCTIONS TO livestock_app;

-- ============================================================================
-- End of Wave 0 init.sql
-- ============================================================================
-- 
-- NOTE: search_path NOT set at role/db level.
-- Runtime: per-module connection string includes "Search Path=<schema>,public"
-- (Karar 4f). EF Core generates schema-qualified SQL (HasDefaultSchema),
-- so search_path is defensive (belt-and-suspenders) for raw Dapper queries.
-- ============================================================================
```

---

## 4c — Seed Data Sırası ve İçeriği

### 1. Country Seed (250 ülke)

**Karar:** 250 ülkenin tamamı seed, `is_active` flag ile launch kontrolü.

| Faz 1 active (~24) | Faz 1 inactive (~226) |
|---|---|
| TR, AZ, KZ, KG, UZ, TM, AE, SA, QA, KW, OM, BH, IQ, EG, MA, TN, DZ, JO, LB, US, GB, DE, FR, NL | kalan ülkeler (`is_active=false`) |

Schema:
```sql
catalog.countries:
  id int PK
  code char(2) UNIQUE NOT NULL
  name_en text, native_name text
  region text
  default_currency_code char(3)
  default_language_code char(2)
  phone_prefix text
  is_active bool DEFAULT false
  display_order int DEFAULT 999
```

### 2. Currency Seed (180)

**Karar:** ISO 4217'nin tamamı seed, ~10 active (TRY, AZN, KZT, USD, EUR, AED, SAR, QAR, GBP, RUB).

Money VO `decimal(18,4)` — ISO 4217'nin tüm minor unit'lerini (0/2/3/4) destekler.

### 3. Language Seed (50)

**Karar:** 50 hedef dil seed, 4 active (TR, EN, AR, RU).

### 4. CertificationType Seed (12)

Health certificate, vaccination card, pedigree, export approval, origin certificate, organic certification, halal certification, quarantine clearance, quality stamp, insurance document, brucellosis test, tuberculosis test.

### 5. Category Seed (~54)

9 ana kategori × ~45 alt = 54. Tree depth 2 (3. seviye Breed'e dönüşür).

```
1. Büyükbaş (livestock-cattle)
   ├─ Süt ineği (livestock-cattle/dairy-cow)
   ├─ Etlik sığır, Damızlık boğa, Düve, Buzağı
2. Küçükbaş
3. Kümes Hayvanları
4. Tek Tırnaklılar (Equines)
5. Arıcılık
6. Yem ve Gübre
7. Tarımsal Makine
8. Tohum ve Fide
9. Veterinerlik
```

### 6. Breed Seed (~80)

Per category seçili ırklar (Holstein, Akkaraman, Saanen, Leghorn, Anadolu arısı, vs.).

### 7. Brand Seed (~100 — Catalog v2 yeni)

Tier 1 brands: Holstein USA, DeLaval, John Deere, Karacabey Merinos, vs. — TR market kritik.

### 8. Location Seed (TR ~885K — Catalog v2 yeni)

5-level hierarchy:
- L1: Türkiye (1)
- L2: 7 Bölge (Marmara, Ege, ...)
- L3: 81 İl
- L4: ~970 İlçe
- L5: ~50,000+ Mahalle/Köy (Faz 1 major cities, full Faz 2)

Stream parser + COPY bulk insert (~5sn TR seed).

### 9. NotificationTemplate Seed (~30 × 4 locale × 2-3 channel = ~250 row)

Wave 6'da. Auth, offer, deal, shipment, verification, subscription, payment, review, marketing, system kategorileri.

### Seed Dosya Formatı — Hibrit

**Karar: JSON fixture + C# loader/seeder**

```
Tools/SeedRunner/
├── SeedRunner.csproj
├── Program.cs              # CLI: --scope all,dev | --module catalog
├── Seeders/
│   ├── IDataSeeder.cs
│   ├── CatalogSeeder.cs
│   ├── IdentitySeeder.cs    # dev only
│   ├── AccountsSeeder.cs    # dev only
│   ├── ListingsSeeder.cs    # dev only
│   └── NotificationsSeeder.cs
└── SeedData/
    ├── catalog/
    │   ├── countries.json
    │   ├── currencies.json
    │   ├── languages.json
    │   ├── certification-types.json
    │   ├── categories.json
    │   ├── breeds.json
    │   ├── brands.json
    │   └── locations/
    │       ├── tr-l1-country.json
    │       ├── tr-l2-regions.json
    │       ├── tr-l3-provinces.json
    │       ├── tr-l4-districts.json
    │       └── tr-l5-villages.json
    └── notifications/
        └── templates.json
```

### JSON Schema Örnekleri

`countries.json`:
```json
[
  {
    "code": "TR",
    "name_en": "Türkiye",
    "native_name": "Türkiye",
    "region": "MENA",
    "default_currency_code": "TRY",
    "default_language_code": "tr",
    "phone_prefix": "+90",
    "is_active": true,
    "display_order": 1
  }
]
```

`categories.json`:
```json
[
  {
    "code": "livestock-cattle",
    "parent_code": null,
    "level": 1,
    "name_translations": { "tr": "Büyükbaş", "en": "Cattle", "ar": "أبقار", "ru": "Крупный рогатый скот" },
    "display_order": 10,
    "icon_key": "cow"
  }
]
```

### Dev vs Prod Seed

```csharp
public enum SeedScope
{
    AllEnvironments,   // Reference data — her yerde
    DevelopmentOnly,   // Test fixtures
    StagingPlus,       // Faz 2 staging fixture
}
```

| Seeder | Scope |
|---|---|
| CatalogSeeder | All |
| NotificationsSeeder (templates) | All |
| IdentitySeeder | DevelopmentOnly (admin@local.test) |
| AccountsSeeder | DevelopmentOnly (3 seller, 2 carrier) |
| ListingsSeeder | DevelopmentOnly (20 sample listing) |

### Idempotent UPSERT + preserveAdminEdits

- Re-run safe — duplicate insert yok
- Admin-edited alanlar (`is_active`, `display_order`, `translations`) seed re-run'da **ezilmiyor**
- Sadece "stable" alanlar (code, ISO standard data) override edilir

```csharp
existing.UpdateStableFieldsFromSeed(seed);
// Inside Country: name_en, region, default_currency_code, default_language_code, phone_prefix
// NOT touched: is_active, display_order, native_name
```

### Prod Admin User

Prod'da seed admin **YOK**. Manuel CLI:
```bash
ASPNETCORE_ENVIRONMENT=Production \
  dotnet run --project Tools/AdminBootstrap -- \
  --email admin@livestock-trading.com \
  --password-from-stdin
```

### SeedRunner CLI

```bash
dotnet run --project Tools/SeedRunner -- \
  --scope all,dev \
  --module catalog,notifications \
  --connection "$LT__DB__APP" \    # App user (Backlog #36 — least privilege)
  --verbose
```

---

## 4d — EF Core Migration Strategy

### Migration Assembly Per Modül

```
Modules/
├── Identity/Identity.Infrastructure/
│   ├── Persistence/Migrations/         ← per modül
│   │   ├── 20260510120000_InitialCreate.cs
│   │   ├── 20260515090000_AddDeviceTokens.cs
│   │   └── IdentityDbContextModelSnapshot.cs
│   └── DesignTime/
│       └── IdentityDbContextDesignTimeFactory.cs
```

### Komut Şablonu

```bash
dotnet ef migrations add <DescriptiveName> \
  --project Modules/<Module>/<Module>.Infrastructure \
  --context <Module>DbContext \
  --output-dir Persistence/Migrations
```

Helper script:
```bash
# scripts/migrations-add.sh
./scripts/migrations-add.sh Identity AddDeviceTokens
```

### DesignTimeFactory Pattern

Her modül `Infrastructure/DesignTime/{Module}DbContextDesignTimeFactory.cs` implement eder. EF tooling design-time DbContext yaratmak için DI'a erişemiyor — factory bağımsız config builder kullanıyor.

```csharp
public sealed class IdentityDbContextDesignTimeFactory : IDesignTimeDbContextFactory<IdentityDbContext>
{
    public IdentityDbContext CreateDbContext(string[] args)
    {
        var configuration = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", optional: true)
            .AddJsonFile("appsettings.Development.json", optional: true)
            .AddEnvironmentVariables(prefix: "LT__")
            .Build();
        
        var connStr = DatabaseConnectionStringBuilder.BuildMigrator(configuration, "identity");
        
        var options = new DbContextOptionsBuilder<IdentityDbContext>()
            .UseNpgsql(connStr, npgsql =>
            {
                npgsql.MigrationsHistoryTable("__EFMigrationsHistory", "identity");
                npgsql.MigrationsAssembly(typeof(IdentityDbContext).Assembly.FullName);
            })
            .UseSnakeCaseNamingConvention()
            .Options;
        
        return new IdentityDbContext(options);
    }
}
```

### Migration Naming Convention

`<Auto-Timestamp>_<DescriptivePascalCase>.cs` — imperative present tense:

✓ `InitialCreate`, `AddRefreshTokenRotation`, `RenameUserPreferences`, `BackfillSellerOnboardingStatus`, `DropDeprecatedAvatarUrlColumn`
✗ `Update`, `Fix1`, `Migration1`, `AddedNewTable` (past tense)

### Forward-Only Production Strategy

**Karar:** Down migration sadece dev'de; prod **forward-fix only**.

| Ortam | Down Migration |
|---|---|
| Dev | ✓ Serbest |
| Staging | ⚠️ Dikkatli |
| Prod | ✗ **Yasak** — Forward-fix only |

### Expand-Contract Pattern (Breaking Schema Change)

```
Aşama 1 (Release N): Expand
  - Yeni column ekle
  - App eski + yeni column'a yazıyor
  - Read eski column'dan

Aşama 2: Backfill
  - Background job: eski → yeni veri kopyala

Aşama 3 (Release N+1): Switchover
  - App sadece yeni column'a yazıyor + okuyor
  - Migration YOK (sadece kod)

Aşama 4 (Release N+2): Contract
  - Eski column drop
```

### Pre-Migration Snapshot

CI/CD pipeline:
```bash
ssh prod "pg_dump -Fc livestock_trading > /backups/pre-deploy-${GIT_SHA}-$(date +%s).dump"
```

### Data Migration

**Inline (≤100k satır, ≤30 sn):**
```csharp
public override void Up(MigrationBuilder b)
{
    b.AddColumn<int>("onboarding_status", "sellers", "accounts", nullable: false, defaultValue: 0);
    b.Sql("""
        UPDATE accounts.sellers 
        SET onboarding_status = CASE 
            WHEN verified_at IS NOT NULL THEN 3
            WHEN suspended_at IS NOT NULL THEN 5
            ELSE 1
        END;
        """);
}
```

**Out-of-band (büyük tablo):** `Tools/DataMigration/` ayrı CLI tool (resumable batch backfill).

---

## 4e — PostGIS Kurulumu

### Extension Placement: public

```sql
CREATE EXTENSION IF NOT EXISTS postgis;
CREATE EXTENSION IF NOT EXISTS pg_trgm;
CREATE EXTENSION IF NOT EXISTS btree_gist;
```

### Version Pin: postgis/postgis:17-3.5

```yaml
services:
  postgres:
    image: postgis/postgis:17-3.5
```

Major.minor pin; patch otomatik.

### NetTopologySuite Integration

```xml
<PackageReference Include="Npgsql.EntityFrameworkCore.PostgreSQL.NetTopologySuite" Version="10.0.x" />
```

**Sadece kullanan modüllerde:**

| Modül | Kullanım |
|---|---|
| Listings | Listing.Location (Point), SavedSearch.RadiusCenter (Point) |
| Accounts | Farm.Location (Point), Seller.CoverageArea (Faz 2 polygon) |
| Carrier | CarrierServiceArea.Area (MultiPolygon — disconnected regions) |
| Catalog | Location.Centroid (Point) — Faz 1 simple; PolygonGeoJsonUrl Faz 2 |
| Marketplace | Deal.PickupLocation, Deal.DropoffLocation |

### SRID 4326 Standard

Storage: SRID 4326 (WGS84, GPS coordinates) only.
Display: Client-side 4326 → 3857 transform (Leaflet/Mapbox handles).

### Geography vs Geometry

| Veri | Tip | SRID | Sebep |
|---|---|---|---|
| Point veri (Location, RadiusCenter, Pickup/Dropoff) | `geography(Point, 4326)` | 4326 | Distance query (meter) hot path |
| Polygon veri (CarrierServiceArea) | `geometry(MultiPolygon, 4326)` | 4326 | Point-in-polygon (`ST_Covers`) |

### GIST Indexes

| Schema.Tablo | Kolon | Index Adı |
|---|---|---|
| `listings.listings` | `location` | `ix_listings_location_gist` |
| `listings.saved_searches` | `radius_center` | `ix_saved_searches_radius_center_gist` |
| `carrier.carrier_service_areas` | `area` | `ix_carrier_service_areas_area_gist` |
| `accounts.farms` | `location` | `ix_farms_location_gist` |
| `catalog.locations` | `centroid` | `ix_locations_centroid_gist` |

### Migration'da Extension YÜKLEME

**Karar: Extension yükleme migration'da DEĞİL — init.sql.** Migrator user'a CREATE EXTENSION yetkisi vermek superuser-level; init.sql tek noktada.

### Testcontainers Setup

```csharp
Container = new PostgreSqlBuilder()
    .WithImage("postgis/postgis:17-3.5")          // PostGIS dahil
    .WithDatabase("livestock_test")
    .WithResourceMapping("init-test.sql", "/docker-entrypoint-initdb.d/")
    .Build();
```

### PostGIS Query Patterns

**ST_DWithin (nearby search):**
```csharp
.Where(l => l.Location.Point.IsWithinDistance(userLocation, radiusMeters))
```

**ST_Covers (carrier service area):**
```csharp
.Where(c => c.ServiceAreas.Any(sa => sa.Area.Covers(pickup)))
```

### Frontend Format

| Veri | Format | Sebep |
|---|---|---|
| Listing.Location (point) | `{ "lat": 41.01, "lng": 28.97 }` | Simple frontend |
| Carrier.ServiceArea (polygon) | GeoJSON MultiPolygon | Leaflet.draw native output |

**Lat/Lng swap test:** NTS `Point(x, y)` = `Point(lng, lat)` — backend mapper'da swap kritik, PR review checklist.

---

## 4f — Connection String Strategy

### Karar: Per-Modül Connection String

```csharp
public static class DatabaseConnectionStringBuilder
{
    public static string BuildApp(IConfiguration cfg, string moduleSchema)
    {
        return new NpgsqlConnectionStringBuilder
        {
            Host = cfg["Database:Host"]!,
            Port = cfg.GetValue<int>("Database:Port", 5432),
            Database = cfg["Database:Name"]!,
            Username = cfg["Database:AppUser"]!,
            Password = cfg["Database:AppPassword"]!,
            SearchPath = $"{moduleSchema},public",
            MaxPoolSize = cfg.GetValue<int>("Database:MaxPoolSize", 15),
            MinPoolSize = cfg.GetValue<int>("Database:MinPoolSize", 2),
            ApplicationName = $"livestock-trading-{moduleSchema}",
        }.ConnectionString;
    }
    
    public static string BuildMigrator(IConfiguration cfg, string moduleSchema) { /* MigratorUser */ }
}
```

**Neden per-modül:**
1. **Search_path zorunluluğu** (Karar 4b hibrit strateji) — her modül kendi schema'sı
2. **Pool izolasyonu** — Npgsql full connection string'i pool key olarak kullanıyor
3. **Resource tuning** — Listings (read-heavy) vs Notifications (write-light) ayrı tunable
4. **Future-proofing** — Listings ileride read replica'ya geçebilir

### Pool Budget

```
1 API instance × 10 modül × 15 max = 150 conn tepe
1 API instance × 10 modül × 2 min = 20 conn idle baseline

PostgreSQL max_connections = 200 (Faz 1)
3 replica Faz 2: max_connections = 500 OR PgBouncer
```

### Configuration

```json
{
  "Database": {
    "Host": "postgres",
    "Port": 5432,
    "Name": "livestock_trading",
    "AppUser": "livestock_app",
    "AppPassword": "${LT__DATABASE__APPPASSWORD}",
    "MigratorUser": "livestock_migrator",
    "MigratorPassword": "${LT__DATABASE__MIGRATORPASSWORD}",
    "MaxPoolSize": 15,
    "MinPoolSize": 2
  }
}
```

### Read Replica Future-Positive

```csharp
public static string BuildApp(IConfiguration cfg, string moduleSchema, DatabaseMode mode = DatabaseMode.Primary)
{
    var hostKey = mode switch
    {
        DatabaseMode.Primary => "Database:Host",
        DatabaseMode.ReadReplica => "Database:ReadReplicaHost",
        _ => "Database:Host"
    };
    var host = cfg[hostKey] ?? cfg["Database:Host"];
    // ...
}
```

Faz 1 ReadReplicaHost null → primary'ye düşüyor.

### Testcontainers Strategy

Per-assembly container, Respawner DML cleanup:

```csharp
[CollectionDefinition("PostgresCollection")]
public class PostgresCollection : ICollectionFixture<PostgresFixture> { }

// Per test:
await _respawner.ResetAsync(_fixture.ConnectionString);
```

Respawner config: Catalog seed korunur (`SchemasToInclude = ["listings", "accounts"]`).

---

## 4g — Database Initialization Workflow

### `migrate-all.sh` Wave-Aware Script

```bash
#!/usr/bin/env bash
set -euo pipefail

: "${LT__DATABASE__HOST:?required}"
: "${LT__DATABASE__MIGRATORUSER:?required}"

readonly FROM_WAVE="${FROM_WAVE:-1}"
readonly ONLY_WAVE="${ONLY_WAVE:-0}"
readonly PARALLEL="${PARALLEL:-1}"

run_migration() {
    local module=$1
    local schema=$(echo "$module" | tr '[:upper:]' '[:lower:]')
    dotnet ef database update \
        --project "Modules/${module}/${module}.Infrastructure" \
        --context "${module}DbContext" \
        --no-build \
        --connection "$(build_conn $schema)"
}

wait_all() {
    local fail=0
    for pid in "$@"; do
        wait "$pid" || fail=1
    done
    return $fail
}

# Wave 1
run_migration "Catalog"

# Wave 2
run_migration "Identity"

# Wave 3+4 (parallel prod, sequential dev)
if [[ "$PARALLEL" == "1" ]]; then
    run_migration "Accounts" & pid_acc=$!
    run_migration "Listings" & pid_lst=$!
    wait_all $pid_acc $pid_lst || exit 1
else
    run_migration "Accounts"
    run_migration "Listings"
fi

# Wave 5 (parallel)
run_migration "Carrier" & pid_car=$!
run_migration "Subscription" & pid_sub=$!
wait_all $pid_car $pid_sub || exit 1

# Wave 6 (parallel)
run_migration "Marketplace" & pid_mkt=$!
run_migration "Messaging" & pid_msg=$!
run_migration "Notifications" & pid_not=$!
wait_all $pid_mkt $pid_msg $pid_not || exit 1

# Wave 7
run_migration "Admin"
```

**FROM_WAVE/ONLY_WAVE:** Resume support — yarıda kalan migration'ı belirli wave'den devam ettir.

### Dev Environment Workflow

```bash
git clone ...
cd livestock-trading
docker compose up -d postgres redis rabbitmq minio
source ./_devops/scripts/dev-env.sh
dotnet build LivestockTrading.sln
PARALLEL=0 ./_devops/scripts/migrate-all.sh        # sıralı dev için
dotnet run --project Tools/SeedRunner -- --scope all,dev
dotnet watch --project src/LivestockTrading.Api
```

### Prod Deployment Workflow

CI/CD 8 stage:

```
[1] Build & Test
[2] Container Build & Push (immutable tag prod-{sha})
[3] Pre-Migration Backup (pg_dump per-schema)
[4] Migrate (FROM_WAVE=1 default)
[5] Seed (--scope all only, no dev fixtures)
[6] Deploy App (rolling Faz 2, all-at-once Faz 1)
[7] Health Check & Smoke Test
[8] Post-Deploy (Sentry release tagging, Slack notification)

Failure handling:
- Build/Push fail → no images, no deploy
- Backup fail → abort
- Migrate fail → app stays at old version
- Seed fail → warn + proceed (non-critical)
- Deploy fail → manual rollback
- Smoke fail → manual rollback trigger
```

### Rollback Senaryoları

| Senaryo | Adımlar | RTO |
|---|---|---|
| **A. Bad code, schema OK** | Image tag previous + compose up | **2 dk** |
| **B. Bad migration** | Hot-fix migration (forward) + image rollback | **30 dk** |
| **C. Bad data migration** | Forward-fix migration / manual SQL | **15 dk** |
| **D. Catastrophic schema** | App stop + DB restore + WAL replay + app start | **60-90 dk** |
| **E. Full DR (host destroyed)** | New host + off-host restore + DNS cutover | **4-8 saat** |

### Multi-Instance Coordination

Faz 1 single instance — race yok.

Faz 2 (`docker-compose.prod.yml`):
```yaml
services:
  migrator:
    image: livestock-api:prod-${GIT_SHA}
    command: ["dotnet", "/app/scripts/migrate-all.dll"]
    restart: "no"
  
  api:
    deploy: { replicas: 3 }
    depends_on:
      migrator: { condition: service_completed_successfully }
```

Migrator one-shot, app waits for completion.

### Health Check Pattern

```csharp
app.MapHealthChecks("/health", new HealthCheckOptions
{
    Predicate = check => check.Tags.Contains("ready"),
    AllowCachingResponses = false
});
app.MapHealthChecks("/live", new HealthCheckOptions
{
    Predicate = _ => false   // process alive only
});
```

### Postmortem Template

Detay: [07-operations.md / Grup C](07-operations.md#grup-c--process--performance--faz-2--dx).

---

## Özet Tablo (Karar 4 tüm alt-kararlar)

| Alt-Karar | Karar |
|---|---|
| **4a — Migration sırası** | Wave 0 (init.sql) → 1 (Catalog) → 2 (Identity) → 3+4 paralel (Accounts+Listings, prod) → 5 paralel (Carrier+Subscription) → 6 paralel (Marketplace+Messaging+Notifications) → 7 (Admin) |
| **4b — Schema-per-modül** | 10 schema (modül adıyla aynı) + public (PostGIS); 2 role (`livestock_app`, `livestock_migrator`); migration history per-schema; hibrit search_path |
| **4c — Seed data** | 250 country + 180 currency + 50 language + 12 cert + ~54 category + ~80 breed + 100 brand + TR 885K location + 30 NotificationTemplate × 4 locale × 2-3 channel; **Hibrit JSON fixture + C# loader**; SeedScope enum; idempotent UPSERT; preserveAdminEdits |
| **4d — EF Core migration** | Per-modül assembly; `IDesignTimeDbContextFactory<T>`; forward-only prod + expand-contract; pre-deploy pg_dump; inline data ≤100k / out-of-band büyük tablo |
| **4e — PostGIS** | `postgis/postgis:17-3.5`; NTS sadece Listings+Carrier+Accounts+Catalog+Marketplace; SRID 4326-only; geography(Point) + geometry(MultiPolygon); GIST index `ix_<table>_<col>_gist`; init.sql extension yükleme |
| **4f — Connection string** | Per-modül (10 ayrı, search_path için); ConnectionStringBuilder template; pool max=15/min=2 per modül; future-positive `mode` (read replica); Testcontainers per-assembly + Respawner |
| **4g — Init workflow** | `migrate-all.sh` FROM_WAVE/ONLY_WAVE resume; CI/CD 8 stage; rollback A-E senaryo RTO; multi-instance migrator service_completed_successfully; `/live` + `/health` ayrımı |
