# Karar 1 — Solution Yapısı

**Status:** FINAL
**Karar tarihi:** Planning session #1

## İlişkili Kararlar

- **Üst:** Yok (foundation karar)
- **Alt:** [Karar 2 (modül listesi)](02-modules-list.md), [Karar 4d (per-modül migration assembly)](04-migration.md#4d-ef-core-migration-strategy), [Karar 4f (per-modül connection string)](04-migration.md#4f-connection-string-strategy)

---

## Karar Özeti

**Seçim: Seçenek B — Her modül ayrı class library, tek host (modüler monolith).**

---

## 3 Seçenek Karşılaştırma

### Seçenek A — Tek Proje, Klasör Bazlı Modüller

```
src/
  Modules/
    Identity/
    Listings/
    Marketplace/
    Messaging/
    Admin/
  Shared/
```

**Artılar:**
- Kurulum/debug süresi düşük — bir proje açar, F5'e basarsın
- Modüller arası refactor kolay (compiler hatasız)
- Küçük ekip için overhead yok

**Eksiler:**
- Modül sınırları derleme zamanında **zorlanmaz** — Identity'ye Listings'ten import kazara girer, kimse fark etmez
- İleride ayrılma gerekirse çok iş çıkar
- Büyük codebase'de namespace disiplini gerektiriyor

### Seçenek B — Her Modül Ayrı Class Library, Tek Host ✓ SEÇİLDİ

```
src/
  Modules/
    LivestockTrading.Identity/        ← .csproj
    LivestockTrading.Listings/        ← .csproj
    LivestockTrading.Marketplace/     ← .csproj
    LivestockTrading.Messaging/       ← .csproj
    LivestockTrading.Admin/           ← .csproj
  LivestockTrading.Api/               ← host, tek .csproj, tek port
  LivestockTrading.Shared/            ← shared kernel, .csproj
```

**Artılar:**
- Modül sınırları **derleme zamanında** kırılıyor — Listings'ten Identity DbContext'e doğrudan erişemezsin, `.csproj` referansı olmalı
- IDE `Show References` / `Go to Definition` modül içinde kalır
- Host tek → deployment karmaşıklığı Seçenek A ile aynı
- İleride modülü microservice'e çıkarmak Seçenek A'ya kıyasla daha az acı

**Eksiler:**
- Solution dosyası ve `dotnet build` süresi biraz artar (önemsiz)
- Cross-modül query gerektiğinde (örn. Admin'den Listings + Identity birlikte) data transfer nesnesi tanımlamak gerekiyor — kasıtlı bir sürtünme, iyi bir şey ama başta alışma süresi var

### Seçenek C — ABP-Tarzı Katmanlı Ayrım

Her modül için: `Domain`, `Application`, `Infrastructure`, `HttpApi` ayrı projeler.

**Artılar:**
- Katman sınırları da derleme zamanında kırılıyor
- Enterprise uyumluluk

**Eksiler:**
- Vertical slice ile **çatışıyor** — vertical slice use-case-per-folder
- Tek bir feature için 4 projede dosya açmak gerekiyor
- Bu projenin ekip büyüklüğüne ve hızına uymuyor

---

## Karar Gerekçesi (Seçenek B)

- Tek host → DevOps Seçenek A kadar basit, deployment aynı
- Derleme zamanı sınır koruması var → ileride "bu modülde kimler konuşuyor?" sorusunun cevabı compiler tarafından veriliyor
- Vertical slice ile uyumlu — her modülün içi feature klasörlerine bölünür, katmanlara değil
- Seçenek C'nin "ABP overhead"i yok

---

## Solution Final Yapısı

Karar 2 sonrası 10 modül için final yapı:

```
LivestockTrading/
├── LivestockTrading.sln
├── src/
│   ├── Modules/
│   │   ├── Catalog/
│   │   │   ├── Catalog.Domain/                  (.csproj)
│   │   │   ├── Catalog.Application/             (.csproj)
│   │   │   └── Catalog.Infrastructure/          (.csproj)
│   │   ├── Identity/
│   │   │   ├── Identity.Domain/
│   │   │   ├── Identity.Application/
│   │   │   └── Identity.Infrastructure/
│   │   ├── Accounts/
│   │   ├── Carrier/
│   │   ├── Listings/
│   │   ├── Marketplace/
│   │   ├── Messaging/
│   │   ├── Notifications/
│   │   ├── Subscription/
│   │   └── Admin/
│   ├── LivestockTrading.Api/                    (tek host)
│   └── Shared/
│       ├── LivestockTrading.Shared.Contracts/   (cross-modül interfaces, DTOs, events)
│       ├── LivestockTrading.Shared.Infrastructure/  (common services)
│       └── LivestockTrading.Shared.Kernel/      (base types, abstractions)
├── Tools/
│   ├── SeedRunner/                              (Karar 4c)
│   ├── OpenApiGen/                              (Karar 6/2)
│   ├── AdminBootstrap/                          (Karar 4c)
│   └── DataMigration/                           (Karar 4d Faz 2)
├── Tests/
│   ├── Unit/                                    (per modül × {Module}.UnitTests)
│   ├── Integration/                             (per modül × {Module}.IntegrationTests)
│   └── EndToEnd/                                (cross-modül flows)
├── _devops/
│   ├── docker/
│   ├── scripts/
│   ├── grafana/
│   ├── prometheus/
│   └── ...
└── _docs/
    └── (planning docs)
```

### Per-Modül 3 Project Pattern (ABP-Lite)

Karar 1'de "Seçenek C reddedildi (vertical slice ile çatışıyor)" denmiş ama pratikte her modül **3 katmana** ayrılıyor:

- `{Module}.Domain` — AR, VO, domain event'ler, domain service
- `{Module}.Application` — Feature klasörleri (vertical slice), handler/validator/endpoint
- `{Module}.Infrastructure` — DbContext, EF configurations, repository, external integrations

**Bu Seçenek C değil:**
- 3 katman per modül, **4 değil** (HttpApi katmanı yok — endpoints Application içinde)
- Vertical slice Application katmanının **iç organizasyonu** (Features/{Feature}/Handler.cs)
- Module sınırı = compile-time, **katman sınırı içeriksel disiplin**

Bu pragmatic split: domain pure logic, application orchestration, infrastructure tech adapter — DDD önerisi.

---

## Modüller Arası İletişim Kuralı

| Durum | Yöntem |
|---|---|
| Aynı transaction, senkron | `IMediator` (MassTransit.Mediator) |
| Cross-modül, senkron okuma | Modül public interface'i (servis inject) — `Shared.Contracts/{Module}/I{Module}ReadService` |
| Cross-modül, asenkron | RabbitMQ event (publish/consume) — `Shared.Events/{Module}/*` |
| Cross-modül, senkron yazma | Sync command interface — `Shared.Contracts/{Module}/I{Module}Commands` (örn. `IListingsCommands.ReserveAsync`, `ICarrierShipmentCommands.CreateAsync`) — gerekçeli istisna |
| Reference data okuma | Catalog modülünün query handler'ı — direkt DbContext paylaşımı yok |

---

## Modül `.csproj` Referans Disiplini

Her modülün `.csproj` dosyası **sadece** şunlara referans verebilir:

```xml
<ItemGroup>
  <!-- ✓ İzinli: Shared -->
  <ProjectReference Include="..\..\Shared\LivestockTrading.Shared.Contracts\LivestockTrading.Shared.Contracts.csproj" />
  <ProjectReference Include="..\..\Shared\LivestockTrading.Shared.Kernel\LivestockTrading.Shared.Kernel.csproj" />
  
  <!-- ✗ YASAK: Başka modülün domain/infrastructure projesi -->
  <!-- <ProjectReference Include="..\..\Modules\Identity\Identity.Domain\..." />  YANLIŞ -->
</ItemGroup>
```

**İstisna:** Sadece `LivestockTrading.Api` host'u tüm modüllerin `.Infrastructure` projelerine referans verir (DI registration için). `Tools/SeedRunner` aynı (deployment-time tool).

Cross-modül kod kullanımı **sadece** `Shared.Contracts` interface'leri üzerinden. Implementation modülün kendi `.Infrastructure`'da, DI ile inject ediliyor.

---

## DevOps İmpact

- Single host → single Docker image (`livestock-api:prod-{sha}`) + worker images
- Tek migration runner per modül (Karar 4d) — `dotnet ef migrations add --project Modules/Listings/Listings.Infrastructure`
- Tek deployment unit Faz 1, paralel API instance Faz 2

Detay: [04-migration.md](04-migration.md), [07-operations.md](07-operations.md).

---

## Özet

| Konu | Karar |
|---|---|
| Solution yapısı | Seçenek B — modüler monolith, ayrı .csproj per modül |
| Per-modül katman | Domain + Application + Infrastructure (3 katman, vertical slice Application içinde) |
| Host | Single `LivestockTrading.Api` |
| Cross-modül iletişim | `Shared.Contracts` interface (sync) + RabbitMQ event (async) |
| `.csproj` referans | Sadece Shared izinli; cross-modül `.csproj` referansı yasak |
| Faz 2 mikroservis ayrımı | Solution Seçenek B'den ayrılma kolay |
